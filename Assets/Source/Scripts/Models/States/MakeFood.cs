using Core;
using Specs;
using UnityEngine;

namespace Models.States
{
    public sealed class MakeFood : IState
    {
        public MakeFood(FoodRecipe foodRecipe)
        {
            _recipe = foodRecipe;

            _cookTimer = _recipe.CookTime;
        }

        private FoodRecipe _recipe = null;
        private float _cookTimer = 5;


        public void Enter(NpcBehaviorLogic controller)
        {

        }

        public void Exit(NpcBehaviorLogic controller)
        {
            _recipe = null;
            Debug.Log("EndWork");
        }

        public void Update(NpcBehaviorLogic controller)
        {
            _cookTimer -= Time.deltaTime;

            if (_cookTimer <= 0)
            {
                BakeryInventory.Add(_recipe.FoodOutput);
                controller.NextState();
            }
        }
    }
}