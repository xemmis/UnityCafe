using Models.Npc;
using UnityEngine;

namespace Models.States
{
    public sealed class QuestState : IdleState
    {
        public bool IsFinished { get; private set; } = true;

        public override void Update(NpcBehaviorLogic controller)
        {
            _waitTimer -= Time.deltaTime;

            if (_waitTimer <= 0)
            {
                controller.SetEmote(Core.EmoteType.Sad, 3.4f);
                IsFinished = false;
                controller.NextState();
            }
        }
    }
}
