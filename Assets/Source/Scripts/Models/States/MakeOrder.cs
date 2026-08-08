using Models.Npc;
using UnityEngine;
using Utils;

namespace Models.States
{
    public class MakeOrder : IdleState
    {
        public MakeOrder(float idleTime = 55)
        {
            _waitTimer = idleTime;
        }

        public override void Update(NpcBehaviorLogic controller)
        {
            _waitTimer -= Time.deltaTime;

            if (_waitTimer <= 0)
            {
                controller.SetEmote(Core.EmoteType.Sad);
                controller.NextState();
            }
        }
    }

    public sealed class WaitOrderState : Specs.IState
    {
        public void Enter(NpcBehaviorLogic controller)
        {
            controller.Interaction?.AssignRandomOrder();
        }

        public void Exit(NpcBehaviorLogic controller) { }

        public void Update(NpcBehaviorLogic controller)
        {
            // Ничего не делаем — ждём, пока AcceptFood не вызовет NextState()
        }
    }
}