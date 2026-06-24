using Specs;
using UnityEngine;

namespace Models.Plant
{
    public sealed class FoodWish : IWish
    {
        public WishType Type => WishType.Food;
        public Sprite Icon { get; private set; }
        public float TimeReduce { get; private set; }

        public FoodWish(Sprite icon, float timeReduce)
        {
            Icon = icon;
            TimeReduce = timeReduce;
        }

        public void Execute(PlantBehavior plant)
        {
            plant.AddGrowthProgress(TimeReduce);
            Debug.Log("Растение полито подкормлено!");
        }
    }
}


namespace Models.Plant
{
}