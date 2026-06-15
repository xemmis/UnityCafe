using Models.Plant;
using UnityEngine;

namespace Specs
{
    public interface IWish
    {
        WishType Type { get; }
        Sprite Icon { get; }
        float TimeReduce { get; }
        void Execute(PlantBehavior plant);
    }
}
