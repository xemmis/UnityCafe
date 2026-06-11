using Core;
using Core.Food;
using Models.Food;
using Models.Npc;
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
            BakeryInventory.Add(_recipe.FoodOutput);
            _recipe = null;
            Debug.Log("EndWork");
        }

        public void Update(NpcBehaviorLogic controller)
        {
            _cookTimer -= Time.deltaTime;

            if (_cookTimer <= 0)
            {
                controller.NextState();
            }
        }
    }
}