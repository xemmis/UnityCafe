using UnityEngine;

namespace Models.Plant
{
    public class WaterWish : IWish
    {
        public WishType Type => WishType.Water;
        public Sprite Icon { get; private set; }
        public float TimeReduce { get; private set; }

        public WaterWish(Sprite icon, float timeReduce)
        {
            Icon = icon;
            TimeReduce = timeReduce;
        }

        public void Execute(PlantBehavior plant)
        {
            plant.AddGrowthProgress(TimeReduce);
            Debug.Log("Растение полито!");
        }
    }
}