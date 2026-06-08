using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "FoodRecipe", menuName = "Food/New Recipe")]
    public sealed class FoodRecipe : ScriptableObject
    {
        [field: SerializeField] public List<IngredientItem> Ingredients { get; private set; } = new();
        [field: SerializeField] public FoodItem FoodOutput { get; private set; }
        [field: SerializeField] public List<NpcAction> NpcActions { get; private set; } = new();
        [field: SerializeField] public float CookTime { get; private set; }
    }
}