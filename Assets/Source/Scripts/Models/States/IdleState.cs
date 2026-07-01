using Models.Npc;
using Specs;
using UnityEngine;

namespace Models.States
{
    public class IdleState : IState
    {
        public IdleState(float idleTime = 5)
        {
            _idleTime = idleTime;
        }

        protected float _idleTime = 0;
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
            _idleTime -= Time.deltaTime;

            if (_idleTime <= 0)
            {
                controller.SetEmote(Core.EmoteType.Happy,3);
                controller.NextState();
            }
        }
    }
}