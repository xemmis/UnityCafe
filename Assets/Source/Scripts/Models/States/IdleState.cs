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

        private float _idleTime = 0;
        private Animator _animator = null;

        public void Enter(NpcBehaviorLogic controller)
        {
            _animator = controller.Animator;
            _animator.SetTrigger("Idle");
        }

        public void Exit(NpcBehaviorLogic controller)
        {
            _animator = null;
        }

        public void Update(NpcBehaviorLogic controller)
        {
            _idleTime -= Time.deltaTime;

            if (_idleTime <= 0)
            {
                controller.NextState();
            }
        }
    }
}