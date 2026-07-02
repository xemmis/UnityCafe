
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
        [SerializeField] private Sprite _npcIcon;
        [SerializeField] private DialogueMood _currentMood = DialogueMood.Good;
        private List<FoodPrefer> _prefers = new();
        private NpcBehaviorLogic _logic;
        public bool IsWaitingFood { get; private set; } = false;

        private void Awake()
        {
            _logic = GetComponent<NpcBehaviorLogic>();
        }

        public void SetWaitCondition(bool condition)
        {
            IsWaitingFood = condition;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            NpcAction currentAction = _logic.CurrentAction;

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
