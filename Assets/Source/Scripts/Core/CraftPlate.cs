using Models;
using Specs;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public sealed class CraftPlate : MonoBehaviour
    {
        private ICraftingVisualizer _visualizer = null;
        private List<ICraftingCell> _craftingCells = new();
        public static CraftPlate Instance { get; private set; } = null;
        private bool _openFlag = false;


        private void Awake()
        {
            InitializeSingleton();

            if (_visualizer == null)
            {
                _visualizer = GetComponentInChildren<ICraftingVisualizer>();
            }
        }

        public void RegisterCraftingCell(ICraftingCell craftingCell)
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
            _openFlag = !_openFlag;

            if (_openFlag)
                _visualizer.Visualize();
            else
            {
                foreach (ICraftingCell cell in _craftingCells)
                    cell.SetIngredient(null);
                _visualizer.Clear();
            }
        }

        public void HandleConfirmCraft()
        {
            FoodRecipe recipe = FoodCraftManager.Instance.CraftFood(GetIngredients());

            if (recipe != null)
            {
                EmployeeManager.SetWork(recipe);
            }
        }

        private List<IngredientItem> GetIngredients()
        {
            List<IngredientItem> ingredientItems = new();

            foreach (ICraftingCell craftingCell in _craftingCells)
            {
                if (!craftingCell.IsEmpty)
                    ingredientItems.Add(craftingCell.GetIngredient());
            }

            return ingredientItems;
        }

        private void OnEnable()
        {
            HandleVisualize();
        }
    }

}