using Core;
using Models.Npc;
using Specs;
using UnityEngine;
using UnityEngine.AI;

namespace Models.States
{
    public sealed class WalkState : IState
    {
        public WalkState(WalkPoint walkPoint)
        {
            _point = walkPoint;
        }

        public WalkState(WalkType walkType)
        {
            _point = WalkManager.Instance?.GetWalkPoint(walkType);
        }

        private WalkPoint _point = new();
        private Animator _animator;
        private NavMeshAgent _agent;

        public void Enter(NpcBehaviorLogic controller)
        {
            _agent = controller.Agent;
            _animator = controller.Animator;
            _agent.SetDestination(_point.transform.position);
        }

        public void Exit(NpcBehaviorLogic controller)
        {
            _point = null;
            _agent = null;
            _animator = null;
        }

        public void Update(NpcBehaviorLogic controller)
        {
            if (_agent == null || !_agent.isOnNavMesh) return;

            bool arrived = !_agent.pathPending
                && _agent.remainingDistance <= _agent.stoppingDistance;

            if (arrived)
                controller.NextState();
        }
    }
}
