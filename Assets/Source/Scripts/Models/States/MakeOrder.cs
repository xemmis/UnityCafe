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
}