using Models.Npc;
using UnityEngine;

namespace Models.States
{
    public sealed class MakeOrder : IdleState
    {
        private Color _transparent = new Color(1, 1, 1, 0);
        private Color _visible = new Color(1, 1, 1, .75f);

        public MakeOrder(float idleTime = 5)
        {
            _idleTime = idleTime;
        }

        public override void Enter(NpcBehaviorLogic controller)
        {
            base.Enter(controller);
            controller.WishImage.color = _visible;
            controller.WishImage.sprite = controller.CurrentAction.FoodRecipe.FoodOutput.Icon;
        }

        public override void Exit(NpcBehaviorLogic controller)
        {
            base.Exit(controller);
            Debug.Log("EXIT");
            controller.WishImage.color = _transparent;
        }
    }
}