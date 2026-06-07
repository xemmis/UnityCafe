using Specs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Models
{
    public sealed class IngredientItem : MonoBehaviour
    {
        [field: SerializeField] public Sprite Icon { get; private set; } = null;
        [field: SerializeField] public FoodPrefer IngredientType { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        private ICraftingCell _currentCell;

        public int Id;
        public override bool Equals(object obj)
        {
            if (obj is not IngredientItem other) return false;
            return Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}