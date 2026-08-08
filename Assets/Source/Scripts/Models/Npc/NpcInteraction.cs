namespace Models.Npc
{
    using Core;
    using Core.Dialogue;
    using Models.Food;
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
        private NpcBehaviorLogic _logic = null;
        private int _questProgress = 0;
        public bool IsWaitingFood { get; private set; } = false;

        private void Awake()
        {
            _logic = GetComponent<NpcBehaviorLogic>();
        }

        public void AddProgress()
        {
            _questProgress++;
        }
        public void ClearProgress()
        {
            _questProgress = 0;
        }

        public void SetWaitCondition(bool condition)
        {
            IsWaitingFood = condition;
        }

        public void Initialize(QuestContainer questContainer)
        {
            _container = questContainer;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            NpcAction currentAction = _logic.CurrentAction;

            if (_container != null && currentAction?.StateType == _container.RequiredState)
            {
                DialogueSystem.Instance.StartDialogue(_container.Data.GetDialogueForProgress(_questProgress));
                return;
            }

            if (currentAction.StateType == StateType.MakeOrder)
            {
                _prefers.Clear();

                _prefers.Add(FoodPrefer.Sweet);
                _prefers.Add(FoodPrefer.Spicy);
                SetWaitCondition(true);
                //TODO Make random foodPrefer and DialogueTree for prefer (need SO container)
            }


            if (currentAction?.Dialogue == null)
                DialogueSystem.Instance.StartDialogue(_currentMood, _npcIcon);
            else
                DialogueSystem.Instance.StartDialogue(currentAction.Dialogue, _npcIcon);
        }

        public void AcceptFood(FoodItem foodItem)
        {
            if (!IsWaitingFood) return;

            int accurancy = 0;

            foreach (FoodPrefer prefer in foodItem.FoodPrefers)
            {
                if (_prefers.Contains(prefer)) accurancy++;
            }

            if (accurancy <= 0)
            {
                _logic.SetEmote(Core.EmoteType.Sad, 100);
                Wallet.AddMoney(foodItem.Cost / 2);
            }

            else if (accurancy == _prefers.Count)
            {
                _logic.SetEmote(EmoteType.Happy, 100);
                Wallet.AddMoney(foodItem.Cost * 2);
            }

            else
            {
                Wallet.AddMoney(foodItem.Cost);
            }

            _logic.NextState();
            IsWaitingFood = false;
        }
    }
}
