using UnityEngine;

namespace Models.Plant
{
    public class AttentionWish : IWish
    {
        public WishType Type => WishType.Attention;
        public Sprite Icon { get; private set; }
        public float TimeReduce { get; private set; }

        public AttentionWish(Sprite icon, float timeReduce)
        {
            Icon = icon;
            TimeReduce = timeReduce;
        }

        public void Execute(PlantBehavior plant)
        {
            plant.AddGrowthProgress(TimeReduce);
            Debug.Log("С растением поиграли!");
        }
    }
}