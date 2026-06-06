using Models;

namespace Specs
{
    public interface IState
    {
        void Enter(NpcBehaviorLogic controller);
        void Exit(NpcBehaviorLogic controller);
        void Update(NpcBehaviorLogic controller);
    }
}
