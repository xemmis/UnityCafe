using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [System.Serializable]
    public sealed class FoodItem
    {
        [field: SerializeField] public List<FoodPrefer> FoodPrefers { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; } = null;
        [field: SerializeField] public string FoodName { get; private set; } = null;
    }


}