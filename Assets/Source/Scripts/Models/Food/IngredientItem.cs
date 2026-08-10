using Models.Npc;
using UnityEngine;

namespace Models.Food
{
    [CreateAssetMenu(fileName = "Ingredient", menuName = "Food/New Ingredient")]
    public sealed class IngredientItem : ScriptableObject
    {
        [field: SerializeField] public Sprite Icon { get; private set; } = null;
        [field: SerializeField] public FoodPrefer IngredientType { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Cost { get; private set; } = 5;
        [field: SerializeField] public int Id { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj is not IngredientItem other) return false;
            return Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}