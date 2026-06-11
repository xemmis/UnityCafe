using Core.Food;

namespace Models.Food
{
    public sealed class IngredientCraftCell : CraftCell
    {
        protected override void Start()
        {
            CraftingPlateVisualizer.Instance.RegisterCell(this);
        }
    }
}