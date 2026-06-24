using Core;
using Models.Npc;
using Specs;

namespace Models.States
{
    public class ExitState : IState
    {
        public void Enter(NpcBehaviorLogic controller)
        {
            controller.Agent.SetDestination(WalkManager.Instance.GetNearestWalkPoint(WalkType.Leave, controller.transform.position).transform.position);
        }

        public void Exit(NpcBehaviorLogic controller) { }

        public void Update(NpcBehaviorLogic controller) { }
    }
}
