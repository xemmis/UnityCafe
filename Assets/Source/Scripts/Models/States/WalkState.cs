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
            Debug.LogWarning(walkPoint == null);
            _point = walkPoint;
        }

        public WalkState(WalkType walkType)
        {
            _point = WalkManager.Instance?.GetFirstFreeWalkPoint(walkType);
        }

        private WalkPoint _point = new();
        private Animator _animator;
        private NavMeshAgent _agent;

        public void Enter(NpcBehaviorLogic controller)
        {
            _agent = controller.Agent;
            _animator = controller.Animator;

            if (_point == null)
            {
                controller.SetEmote(EmoteType.Dissapointed, 100);
                controller.NextState();
                return;
            }

            controller.SetEmote(EmoteType.Happy);
            if (_point.Type != WalkType.Leave)
                _point.Reserve();

            _agent.SetDestination(_point.transform.position);
        }

        public void Exit(NpcBehaviorLogic controller)
        {
            if (_point != null && _point.Type != WalkType.Leave)
                _point.CancelReservation();

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
