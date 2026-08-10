namespace Models.Npc
{
    using Core;
    using Core.Dialogue;
    using Models.Food;
    using Models.States;
    using Specs;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.UI;

    public sealed class NpcBehaviorLogic : MonoBehaviour
    {
        [SerializeField] private ActionsSO _npcSO = null;
        [SerializeField] private bool _isEmploye = false;
        [SerializeField] private Image _emoteSprite = null;

        private EmoteContainer _container = null;
        private readonly Queue<IState> _stateQueue = new();
        private NavMeshAgent _agent;
        private Animator _animator;
        private IState _currentState;
        private bool _isWorking;
        public bool IsWorking => _isWorking;
        public event System.Action<NpcBehaviorLogic> OnDespawn;
        public NpcAction CurrentAction { get; private set; }
        public NavMeshAgent Agent => _agent;
        public Animator Animator => _animator;
        private NpcVisualizer _visualizer = null;
        private NpcInteraction _interaction = null;
        public IState CurrentState => _currentState;
        public NpcInteraction Interaction => _interaction;

        private void Awake()
        {
            TryGetComponent(out _agent);
            TryGetComponent(out _animator);
            TryGetComponent(out _interaction);
            _visualizer = new NpcVisualizer(_emoteSprite, this);
        }

        private void Start()
        {
            if (_isEmploye)
                EmployeeManager.RegisterEmployee(this);
        }
        public void Initialize(ActionsSO npcSO, EmoteContainer emoteContainer = null, QuestContainer questContainer = null)
        {
            _npcSO = npcSO;
            _container = emoteContainer;

            _interaction?.ResetForSpawn(questContainer);

            _visualizer.Initialize(emoteContainer);
            int rand = Random.Range(0, npcSO.Actions.Count);
            InitializeActions(npcSO.Actions[rand].Actions);
        }

        public void InitializeActions(List<NpcAction> actions)
        {
            foreach (NpcAction npcAction in actions)
            {
                IState state = NpcStateFabric.CreateState(npcAction);
                _stateQueue.Enqueue(state);
            }
            NextState();
        }

        public void SetWorkState(bool condition, FoodRecipe recipe = null)
        {
            _isWorking = condition;

            if (recipe == null) return;

            InitializeActions(recipe.NpcActions);
        }

        public void SetEmote(EmoteType type, float chance = 45f, float duration = 5f)
        {
            if (Random.Range(0f, 100f) > chance)
            {
                return;
            }

            _visualizer.SetEmote(type);
            _visualizer.ClearAfterDelay(duration);
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
                return;
            }

            _isWorking = false;
            ApplyState(_isEmploye ? new IdleState() : new ExitState());
        }


        // В ApplyState когда доходит до ExitState:
        private void ApplyState(IState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }

        public void ReturnToPool()
        {
            OnDespawn?.Invoke(this);
        }

        private void FixedUpdate()
        {
            _currentState?.Update(this);
        }
    }
}
 