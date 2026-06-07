using Models;
using Specs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    public sealed class FoodCraftManager : MonoBehaviour
    {
        public static FoodCraftManager Instance = null;
        private Dictionary<FoodRecipe, FoodItem> _foodDict = new();

        public void RegisterRecipe(FoodRecipe recipe)
        {
            _foodDict.Add(recipe, recipe.FoodOutput);
        }

        public FoodRecipe CraftFood(List<IngredientItem> ingredients)
        {
            foreach (var (recipe, foodItem) in _foodDict)
            {
                if (RecipeMatchesIngredients(recipe, ingredients))
                    return recipe;
            }

            return null;
        }

        private bool RecipeMatchesIngredients(FoodRecipe recipe, List<IngredientItem> ingredients)
        {
            if (recipe.Ingredients.Count != ingredients.Count)
                return false;

            // Сортируем оба списка по ID/имени, чтобы порядок не влиял на результат
            var recipeIngredients = recipe.Ingredients.OrderBy(i => i.Id).ToList();
            var inputIngredients = ingredients.OrderBy(i => i.Id).ToList();

            return recipeIngredients.SequenceEqual(inputIngredients);
        }

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (FoodCraftManager.Instance == null)
            {
                FoodCraftManager.Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

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
                _visualizer.Clear();
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
    }

}