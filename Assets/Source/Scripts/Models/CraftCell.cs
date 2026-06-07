using Core;
using Specs;
using UnityEngine;

namespace Models
{
    public class CraftCell : MonoBehaviour, ICraftingCell
    {
        public bool IsEmpty { get; set; }
        private IngredientItem _currentIngredient = null;

        protected virtual void Start()
        {
            CraftPlate.Instance.RegisterCraftingCell(this);
        }

        public IngredientItem GetIngredient()
        {
            return _currentIngredient;
        }

        public void SetIngredient(IngredientItem ingredient)
        {
            _currentIngredient = ingredient;
            IsEmpty = false;
        }

        public void ClearCell()
        {
            IsEmpty = true;
        }
    }
}