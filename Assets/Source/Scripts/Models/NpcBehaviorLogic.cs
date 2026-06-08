using Core.Dialogue;
using Core;
using Specs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Models.States;

namespace Models
{
    public sealed class NpcBehaviorLogic : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite _npcIcon = null;
        [SerializeField] private bool _isEmploye = false;
        private readonly Queue<IState> _stateQueue = new();
        private NavMeshAgent _agent = null;
        private Animator _animator = null;
        private ActionsSO _npcSO = null;
        private NpcAction _currentAction = null;
        private IState _currentState = null;
        private DialogueMood _currentMood;

        public NavMeshAgent Agent => _agent;
        public Animator Animator => _animator;

        private void Awake()
        {
            SetRandomMood();

            InitializeComponents();
        }

        private void Start()
        {
            if (_isEmploye)
                EmployeeManager.RegisterEmployee(this);
        }

        private void InitializeComponents()
        {
            TryGetComponent<NavMeshAgent>(out _agent);
            TryGetComponent<Animator>(out _animator);
        }

        private void SetRandomMood()
        {
            var moods = (DialogueMood[])System.Enum.GetValues(typeof(DialogueMood));

            _currentMood = moods[Random.Range(0, moods.Length)];
        }

        public bool HasActions()
        {
            return _currentAction != null;
        }

        public void Initialize(ActionsSO npcSO)
        {
            _npcSO = npcSO;
            int rand = Random.Range(0, npcSO.Actions.Count - 1);

            foreach (NpcAction npcAction in npcSO.Actions[rand].Actions)
            {
                IState state = NpcStateFabric.CreateState(npcAction);
                _stateQueue.Enqueue(state);
            }
        }

        public void Initialize(List<NpcAction> actions)
        {
            foreach (NpcAction npcAction in actions)
            {
                IState state = NpcStateFabric.CreateState(npcAction);
                _stateQueue.Enqueue(state);
            }
            NextState();
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

            if (!_isEmploye)
            {
                _stateQueue.Clear();
                _stateQueue.Enqueue(new ExitState());
                ApplyState(_stateQueue.Dequeue());
            }
        }

        private void ApplyState(IState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            print(newState.GetType());
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
                DialogueSystem.Instance.StartDialogue(_currentMood, _npcIcon);
            }
            else
            {
                DialogueSystem.Instance.StartDialogue(_currentAction.Dialogue, _npcIcon);
            }
        }
    }
}