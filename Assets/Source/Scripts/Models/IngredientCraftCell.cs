using Core;

namespace Models
{
    public sealed class IngredientCraftCell : CraftCell
    {
        protected override void Start()
        {
            CraftingPlateVisualizer.Instance.RegisterCell(this);
        }
    }
}