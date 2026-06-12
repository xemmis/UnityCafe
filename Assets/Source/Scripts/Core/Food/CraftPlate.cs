using Models.Food;
using Specs;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Food
{
    public sealed class CraftPlate : MonoBehaviour
    {
        private ICraftingVisualizer _visualizer = null;
        private List<IUICell<IngredientItem>> _craftingCells = new();
        public static CraftPlate Instance { get; private set; } = null;

        private void Awake()
        {
            InitializeSingleton();

            if (_visualizer == null)
            {
                _visualizer = GetComponentInChildren<ICraftingVisualizer>();
            }
        }

        public void RegisterCraftingCell(IUICell<IngredientItem> craftingCell)
        {
            _craftingCells.Add(craftingCell);
        }

        private void InitializeSingleton()
        {
            if (CraftPlate.Instance == null)
            {
                CraftPlate.Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void HandleVisualize()
        {
            // Не трогаем свой _openFlag — доверяем флагу визуалайзера
            _visualizer.Visualize();

            foreach (IUICell<IngredientItem> cell in _craftingCells)
                cell.SetItem(null);

        }

        public void ForceClose()
        {
            foreach (IUICell<IngredientItem> cell in _craftingCells)
                cell.SetItem(null);
            _visualizer.Clear();
        }

        public void HandleConfirmCraft()
        {
            List<IngredientItem> ingredients = GetIngredients();
            FoodRecipe recipe = FoodCraftManager.Instance.CraftFood(ingredients);

            if (recipe != null)
            {
                foreach (IngredientItem ingredient in ingredients)
                    BakeryInventory.Remove(ingredient);

                RemoveIngredients();
                EmployeeManager.SetWork(recipe);
                return;
            }
            print("NE");
        }
        private void RemoveIngredients()
        {
            foreach (IUICell<IngredientItem> cell in _craftingCells)
            {
                cell.SetItem(null);
            }
        }

        private List<IngredientItem> GetIngredients()
        {
            List<IngredientItem> ingredientItems = new();

            foreach (IUICell<IngredientItem> craftingCell in _craftingCells)
            {
                if (!craftingCell.IsEmpty)
                    ingredientItems.Add(craftingCell.GetItem());
            }

            return ingredientItems;
        }
    }

}