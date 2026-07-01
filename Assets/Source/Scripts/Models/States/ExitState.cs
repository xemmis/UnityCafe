using Core;
using Models.Npc;
using Specs;
using UnityEngine.AI;

namespace Models.States
{
    public class ExitState : IState
    {
        private NavMeshAgent _agent;

        public void Enter(NpcBehaviorLogic controller)
        {
            _agent = controller.Agent;
            _agent.SetDestination(WalkManager.Instance.GetNearestWalkPoint(WalkType.Leave, controller.transform.position).transform.position);
        }

        public void Exit(NpcBehaviorLogic controller)
        {
            controller.ReturnToPool();
            _agent = null;
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