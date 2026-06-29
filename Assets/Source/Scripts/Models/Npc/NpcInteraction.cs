
namespace Models.Npc
{
    using Core.Dialogue;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(NpcBehaviorLogic))]
    public sealed class NpcInteraction : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite _npcIcon;
        [SerializeField] private DialogueMood _currentMood = DialogueMood.Good;

        private NpcBehaviorLogic _logic;

        private void Awake()
        {
            _logic = GetComponent<NpcBehaviorLogic>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            NpcAction currentAction = _logic.CurrentAction;

            if (currentAction?.Dialogue == null)
                DialogueSystem.Instance.StartDialogue(_currentMood, _npcIcon);
            else
                DialogueSystem.Instance.StartDialogue(currentAction.Dialogue, _npcIcon);
        }
    }
}
