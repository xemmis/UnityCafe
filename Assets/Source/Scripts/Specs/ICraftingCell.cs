using Models;

namespace Specs
{
    public interface ICraftingCell
    {
        bool IsEmpty { get; set; }
        void SetIngredient(IngredientItem ingredient);
        void ClearCell();
        IngredientItem GetIngredient();
    }
}
