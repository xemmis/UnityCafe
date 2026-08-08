using Models.Npc;
using Specs;
using UnityEngine;

namespace Models.States
{
    public class IdleState : IState
    {
        public IdleState(float idleTime = 5)
        {
            _waitTimer = idleTime;
        }

        protected float _waitTimer = 0;
        protected Animator _animator = null;

        public virtual void Enter(NpcBehaviorLogic controller)
        {
            _animator = controller.Animator;
            _animator.SetTrigger("Idle");
        }

        public virtual void Exit(NpcBehaviorLogic controller)
        {
            _animator = null;
        }

        public virtual void Update(NpcBehaviorLogic controller)
        {
            _waitTimer -= Time.deltaTime;

            if (_waitTimer <= 0)
            {
                controller.SetEmote(Core.EmoteType.Happy,3);
                controller.NextState();
            }
        }
    }
}