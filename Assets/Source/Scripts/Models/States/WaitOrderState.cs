using Models.Npc;

namespace Models.States
{
    public sealed class WaitOrderState : Specs.IState
    {
        public void Enter(NpcBehaviorLogic controller)
        {
            controller.Interaction?.AssignOrder();
        }

        public void Exit(NpcBehaviorLogic controller) { }

        public void Update(NpcBehaviorLogic controller)
        {        }
    }
}