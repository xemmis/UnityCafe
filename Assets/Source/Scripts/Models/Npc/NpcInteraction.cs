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
        [SerializeField] private QuestContainer _container = null;

        private List<FoodPrefer> _prefers = new();
        private DialogueTree _orderHint = null;
        private NpcBehaviorLogic _logic = null;
        private int _questProgress = 0;
        public bool IsWaitingFood { get; private set; } = false;

        private void Awake()
        {
            _logic = GetComponent<NpcBehaviorLogic>();
        }

        public void AddProgress() => _questProgress++;
        public void ClearProgress() => _questProgress = 0;
        public void SetWaitCondition(bool condition) => IsWaitingFood = condition;

        public void Initialize(QuestContainer questContainer)
        {
            _container = questContainer;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_container != null && _logic.CurrentState is QuestState)
            {
                DialogueSystem.Instance.StartDialogue(_container.Data.GetDialogueForProgress(_questProgress));
                return;
            }

            if (_logic.CurrentState is MakeOrder)
            {
                _logic.ChangeState(new WaitOrderState());
                return;
            }

            if (_logic.CurrentState is WaitOrderState)
            {
                if (_orderHint != null)
                    DialogueSystem.Instance.StartDialogue(_orderHint, _npcIcon);
                return;
            }

            DialogueSystem.Instance.StartDialogue(_currentMood, _npcIcon);
        }

        // Вызывается из WaitOrderState.Enter при переходе в состояние ожидания заказа
        public void AssignRandomOrder()
        {
            var (prefer, hint) = OrderFactory.Instance.CreateRandomOrder();

            _prefers.Clear();
            _prefers.Add(prefer);
            _orderHint = hint;

            SetWaitCondition(true);

            if (hint != null)
                DialogueSystem.Instance.StartDialogue(hint, _npcIcon);
        }

        public void AcceptFood(FoodItem foodItem)
        {
            if (!IsWaitingFood || foodItem == null) return;

            int accurancy = 0;

            if (foodItem.FoodPrefers != null)
            {
                foreach (FoodPrefer prefer in foodItem.FoodPrefers)
                    if (_prefers.Contains(prefer)) accurancy++;
            }

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

            IsWaitingFood = false;
            _orderHint = null;
            _logic.NextState();
        }
    }
}