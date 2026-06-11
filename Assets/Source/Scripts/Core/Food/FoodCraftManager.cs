using Models.Food;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Food
{
    public sealed class FoodCraftManager : MonoBehaviour
    {
        [SerializeField] private List<IngredientItem> _startIngredients = new();
        [SerializeField] private List<FoodRecipe> _recipes = new();
        private Dictionary<FoodRecipe, FoodItem> _foodDict = new();

        public static FoodCraftManager Instance { get; private set; } = null;

        private void Start()
        {
            foreach (IngredientItem ingredientItem in _startIngredients)
            {
                BakeryInventory.Add(ingredientItem);
            }

            foreach (FoodRecipe foodRecipe in _recipes)
            {
                RegisterRecipe(foodRecipe);
            }
        }

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

}