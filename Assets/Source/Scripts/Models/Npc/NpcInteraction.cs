namespace Models.Npc
{
    using Core;
    using Core.Dialogue;
    using Models.Food;
    using Models.States;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(NpcBehaviorLogic))]
    public sealed class NpcInteraction : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite _npcIcon = null;

        private QuestContainer _container = null;
        private QuestDialoguePair? _activeQuestPair = null;
        private bool _isQuestOrder = false;

        private List<FoodPrefer> _prefers = new();
        private DialogueTree _orderHint = null;
        private NpcBehaviorLogic _logic = null;
        private NpcMood _mood = null;

        public bool IsWaitingFood { get; private set; } = false;
        public DialogueMood CurrentMood => _mood.Current;

        private void Awake()
        {
            _logic = GetComponent<NpcBehaviorLogic>();

            // Safety-net для NPC, которые не проходят через NpcFabric
            // (например, статично размещённые сотрудники) — у них не будет
            // вызова ResetForSpawn, поэтому настроение должно быть валидно
            // сразу после Awake.
            _mood = NpcMood.CreateRandom();
            _mood.OnMoodChanged += HandleMoodChanged;
        }

        public void SetWaitCondition(bool condition) => IsWaitingFood = condition;

        /// <summary>
        /// Единая точка сброса состояния при каждом "спавне" из пула.
        /// Раньше квест назначался только если questContainer != null,
        /// из-за чего переиспользованный NPC мог унаследовать чужой квест.
        /// </summary>
        public void ResetForSpawn(QuestContainer questContainer)
        {
            _container = questContainer;
            _activeQuestPair = null;
            _isQuestOrder = false;
            IsWaitingFood = false;
            _prefers.Clear();
            _orderHint = null;

            _mood = NpcMood.CreateRandom();
            _mood.OnMoodChanged += HandleMoodChanged;
        }

        private void GrantQuestReward(QuestDialoguePair pair)
        {
            if (pair.RewardPlant == null) return;

            SeedInventory.Add(pair.RewardPlant);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_logic.CurrentState is MakeOrder)
            {
                _logic.ChangeState(new WaitOrderState());
                return;
            }

            if (_logic.CurrentState is WaitOrderState)
            {
                DialogueTree hint = _isQuestOrder
                    ? _activeQuestPair?.Dialogue
                    : _orderHint;

                if (hint != null)
                    DialogueSystem.Instance.StartDialogue(hint, _npcIcon);
                return;
            }

            DialogueSystem.Instance.StartDialogue(_mood.Current, _npcIcon);
        }

        // Вызывается из WaitOrderState.Enter
        public void AssignOrder()
        {
            if (_container != null && _container.Data.HasNextStage())
                AssignQuestOrder();
            else
                AssignRandomOrder();
        }

        private void AssignQuestOrder()
        {
            QuestDialoguePair? pair = _container.Data.GetCurrentPair();

            if (!pair.HasValue)
            {
                AssignRandomOrder();
                return;
            }

            _activeQuestPair = pair;
            _isQuestOrder = true;

            SetWaitCondition(true);

            if (pair.Value.Dialogue != null)
                DialogueSystem.Instance.StartDialogue(pair.Value.Dialogue, _npcIcon);
        }

        private void AssignRandomOrder()
        {
            var (prefer, hint) = OrderFactory.Instance.CreateRandomOrder();

            _prefers.Clear();
            _prefers.Add(prefer);
            _orderHint = hint;
            _isQuestOrder = false;

            SetWaitCondition(true);

            if (hint != null)
                DialogueSystem.Instance.StartDialogue(hint, _npcIcon);
        }

        public void AcceptFood(FoodItem foodItem)
        {
            if (!IsWaitingFood || foodItem == null) return;

            if (_isQuestOrder)
                HandleQuestFood(foodItem);
            else
                HandleRegularFood(foodItem);

            IsWaitingFood = false;
            _orderHint = null;
            _logic.NextState();
        }

        private void HandleQuestFood(FoodItem foodItem)
        {
            bool isCorrect = _activeQuestPair.HasValue
                && _activeQuestPair.Value.RequiredItem == foodItem;

            if (isCorrect)
            {
                _logic.SetEmote(EmoteType.Happy, 100);
                Wallet.AddMoney(foodItem.Cost * 2);
                QuestSystem.Instance?.CompleteQuestStage(_container);
                _mood.Improve();
            }
            else
            {
                _logic.SetEmote(EmoteType.Sad, 100);
                Wallet.AddMoney(foodItem.Cost / 2);
                _mood.Worsen();
            }

            _activeQuestPair = null;
            _isQuestOrder = false;
        }

        private void HandleRegularFood(FoodItem foodItem)
        {
            int accurancy = 0;

            if (foodItem.FoodPrefers != null)
                foreach (FoodPrefer prefer in foodItem.FoodPrefers)
                    if (_prefers.Contains(prefer)) accurancy++;

            if (accurancy <= 0)
            {
                _logic.SetEmote(EmoteType.Sad, 100);
                Wallet.AddMoney(foodItem.Cost / 2);
                _mood.Worsen();
            }
            else if (accurancy >= _prefers.Count)
            {
                _logic.SetEmote(EmoteType.Happy, 100);
                Wallet.AddMoney(foodItem.Cost * 2);
                _mood.Improve();
            }
            else
            {
                // Частичное совпадение предпочтений — не хорошо и не плохо
                Wallet.AddMoney(foodItem.Cost);
            }
        }

        // Вызывается из WalkState при поиске свободного столика
        public void NotifyFoundFreeTable() => _mood.Improve();
        public void NotifyNoFreeTable() => _mood.Worsen();

        private void HandleMoodChanged(DialogueMood mood)
        {
            EmoteType emote = mood switch
            {
                DialogueMood.Good => EmoteType.Happy,
                DialogueMood.Bad => EmoteType.Angry,
                _ => EmoteType.None
            };

            if (emote != EmoteType.None)
                _logic.SetEmote(emote, 100);
        }
    }
}