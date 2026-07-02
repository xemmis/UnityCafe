using Models.Npc;
using UnityEngine;
using Utils;

namespace Models.States
{
    public sealed class MakeOrder : IdleState
    {
        public MakeOrder(float idleTime = 5)
        {
            _idleTime = idleTime;
        }

        public override void Enter(NpcBehaviorLogic controller)
        {
            base.Enter(controller);
        }

        public override void Update(NpcBehaviorLogic controller)
        {
            _idleTime -= Time.deltaTime;

            if (_idleTime <= 0)
            {
                controller.SetEmote(Core.EmoteType.Sad);
                controller.NextState();
            }
        }

        public override void Exit(NpcBehaviorLogic controller)
        {
            base.Exit(controller);
        }
    }
}