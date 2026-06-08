using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "Food", menuName = "Food/New Food")]
    public sealed class FoodItem : ScriptableObject
    {
        [field: SerializeField] public List<FoodPrefer> FoodPrefers { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; } = null;
        [field: SerializeField] public string FoodName { get; private set; } = null;


        public void TakePrefers(List<FoodPrefer> prefers)
        {
            FoodPrefers = prefers;
        }
    }
}