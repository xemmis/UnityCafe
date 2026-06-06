using Core;
using Specs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Models
{
    public class NpcBehaviorLogic : MonoBehaviour, IPointerClickHandler
    {
        private readonly Queue<IState> _stateQueue = new();
        private IState _currentState = null;
        private NpcSO _npcSO = null;
        private NpcAction _currentAction = null;
        private DialogueMood _currentMood;

        private void Awake()
        {
            SetRandomMood();
        }

        private void SetRandomMood()
        {
            var moods = (DialogueMood[])System.Enum.GetValues(typeof(DialogueMood));

            _currentMood = moods[Random.Range(0, moods.Length)];
        }

        public void Initialize(NpcSO npcSO)
        {
            _npcSO = npcSO;

            foreach (NpcAction npcAction in npcSO.Actions)
            {
                IState state = NpcStateFabric.CreateState(npcAction.StateType);
                _stateQueue.Enqueue(state);
            }
        }

        public void ChangeState(IState newState)
        {
            _stateQueue.Clear();
            ApplyState(newState);
        }

        public void NextState()
        {
            if (_stateQueue.Count > 0)
            {
                ApplyState(_stateQueue.Dequeue());
            }
            else
            {
                _stateQueue.Clear();
                // TODO EXITSTATE!!
                // ApplyState(_stateQueue.Dequeue());
            }
        }

        private void ApplyState(IState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }

        public void FixedUpdate()
        {
            _currentState?.Update(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_currentAction.Dialogue == null)
            {
                DialogueSystem.Instance.StartDialogue(_currentMood, _npcSO.NpcIcon);
            }
            else
            {
                DialogueSystem.Instance.StartDialogue(_currentAction.Dialogue, _npcSO.NpcIcon);
            }
        }
    }
}