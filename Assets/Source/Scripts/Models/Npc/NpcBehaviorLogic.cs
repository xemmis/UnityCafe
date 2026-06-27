namespace Models.Npc
{
    using Core;
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

        private readonly Queue<IState> _stateQueue = new();
        private NavMeshAgent _agent;
        private Animator _animator;
        private IState _currentState;
        private bool _isWorking;

        public bool IsWorking => _isWorking;
        public NpcAction CurrentAction { get; private set; }
        public NavMeshAgent Agent => _agent;
        public Animator Animator => _animator;

        private NpcVisualizer _visualizer;

        private void Awake()
        {
            TryGetComponent(out _agent);
            TryGetComponent(out _animator);
            _visualizer = new NpcVisualizer(GetComponentInChildren<Image>());
        }

        private void Start()
        {
            if (_isEmploye)
                EmployeeManager.RegisterEmployee(this);

            Initialize(_npcSO, _visualizer.EmoteContainer);
        }

        public void Initialize(ActionsSO npcSO, EmoteContainer emoteContainer = null)
        {
            _npcSO = npcSO;
            int rand = Random.Range(0, npcSO.Actions.Count);

            InitializeActions(npcSO.Actions[rand].Actions);

            _visualizer.Initialize(emoteContainer);
        }

        private void InitializeActions(List<NpcAction> actions)
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

        public void SetEmote(EmoteType type, float chance = 100f)
        {
            if (Random.Range(0f, 100f) <= chance)
                _visualizer.SetEmote(type);
        }
        public void SetWish(Sprite sprite)
        {
            _visualizer.SetSprite(sprite);
        }
        public void ClearEmote() => _visualizer.ClearEmote();

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
            ClearEmote();
            ApplyState(_isEmploye ? new IdleState() : new ExitState());
        }

        private void ApplyState(IState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }

        private void FixedUpdate()
        {
            _currentState?.Update(this);
        }
    }
}