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
        [SerializeField] private DialogueMood _currentMood = DialogueMood.Good;

        private QuestContainer _container = null;
        private QuestDialoguePair? _activeQuestPair = null;
        private bool _isQuestOrder = false;

        private List<FoodPrefer> _prefers = new();
        private DialogueTree _orderHint = null;
        private NpcBehaviorLogic _logic = null;
        public bool IsWaitingFood { get; private set; } = false;

        private void Awake()
        {
            _logic = GetComponent<NpcBehaviorLogic>();
        }

        public void SetWaitCondition(bool condition) => IsWaitingFood = condition;

        public void Initialize(QuestContainer questContainer)
        {
            _container = questContainer;
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

            DialogueSystem.Instance.StartDialogue(_currentMood, _npcIcon);
        }

        // Вызывается из WaitOrderState.Enter при переходе в состояние ожидания заказа.
        // Решает, обычный это заказ или квестовый.
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
            }
            else
            {
                _logic.SetEmote(EmoteType.Sad, 100);
                Wallet.AddMoney(foodItem.Cost / 2);
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
            }
            else if (accurancy >= _prefers.Count)
            {
                _logic.SetEmote(EmoteType.Happy, 100);
                Wallet.AddMoney(foodItem.Cost * 2);
            }
            else
            {
                Wallet.AddMoney(foodItem.Cost);
            }
        }
    }
}