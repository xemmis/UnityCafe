using Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public static class BakeryInventory
    {
        private static readonly Dictionary<FoodItem, int> _foodInventory = new();
        private static readonly Dictionary<IngredientItem, int> _ingredientInventory = new();

        public static IReadOnlyDictionary<FoodItem, int> FoodInventory => _foodInventory;
        public static IReadOnlyDictionary<IngredientItem, int> IngredientInventory => _ingredientInventory;

        public static event Action<FoodItem, int> OnFoodInventoryChange;
        public static event Action<IngredientItem, int> OnIngredientInventoryChange;

        // ==================== Количество ====================

        public static int GetAmount(FoodItem item) =>
            item != null && _foodInventory.TryGetValue(item, out int amount) ? amount : 0;

        public static int GetAmount(IngredientItem item) =>
            item != null && _ingredientInventory.TryGetValue(item, out int amount) ? amount : 0;

        // ==================== Добавление ====================

        public static void Add(FoodItem item, int amount = 1)
        {
            if (!ValidateInput(item, amount, "добавить")) return;

            _foodInventory[item] = GetAmount(item) + amount;
            OnFoodInventoryChange?.Invoke(item, _foodInventory[item]);
            Debug.Log($"[Inventory] +{amount} {item.FoodName}. Итого: {_foodInventory[item]}");
        }

        public static void Add(IngredientItem item, int amount = 1)
        {
            if (!ValidateInput(item, amount, "добавить")) return;

            _ingredientInventory[item] = GetAmount(item) + amount;
            OnIngredientInventoryChange?.Invoke(item, _ingredientInventory[item]);
            Debug.Log($"[Inventory] +{amount} {item.Name}. Итого: {_ingredientInventory[item]}");
        }

        // ==================== Удаление ====================

        public static bool Remove(FoodItem item, int amount = 1)
        {
            if (!ValidateInput(item, amount, "удалить")) return false;

            int current = GetAmount(item);
            if (current < amount)
            {
                Debug.LogWarning($"[Inventory] Не хватает {item.FoodName}: нужно {amount}, есть {current}");
                return false;
            }

            _foodInventory[item] = current - amount;
            if (_foodInventory[item] == 0) _foodInventory.Remove(item);

            OnFoodInventoryChange?.Invoke(item, GetAmount(item));
            Debug.Log($"[Inventory] -{amount} {item.FoodName}. Осталось: {GetAmount(item)}");
            return true;
        }

        public static bool Remove(IngredientItem item, int amount = 1)
        {
            if (!ValidateInput(item, amount, "удалить")) return false;

            int current = GetAmount(item);
            if (current < amount)
            {
                Debug.LogWarning($"[Inventory] Не хватает {item.Name}: нужно {amount}, есть {current}");
                return false;
            }

            _ingredientInventory[item] = current - amount;
            if (_ingredientInventory[item] == 0) _ingredientInventory.Remove(item);

            OnIngredientInventoryChange?.Invoke(item, GetAmount(item));
            Debug.Log($"[Inventory] -{amount} {item.Name}. Осталось: {GetAmount(item)}");
            return true;
        }

        // ==================== Проверка ингредиентов ====================

        /// <summary>
        /// Проверяет наличие всех ингредиентов (по 1 штуке каждого).
        /// </summary>
        public static bool HasAllIngredients(IEnumerable<IngredientItem> required)
        {
            if (required == null) return false;

            foreach (var item in required)
            {
                if (GetAmount(item) <= 0)
                {
                    Debug.Log($"[Inventory] Не хватает: {item.Name}");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Проверяет наличие ингредиентов с учётом нужного количества.
        /// </summary>
        public static bool HasAllIngredients(Dictionary<IngredientItem, int> required)
        {
            if (required == null || required.Count == 0) return false;

            foreach (var (item, need) in required)
            {
                int have = GetAmount(item);
                if (have < need)
                {
                    Debug.Log($"[Inventory] Не хватает {item.Name}: нужно {need}, есть {have}");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Снимает все ингредиенты из рецепта. Если чего-то не хватает — ничего не трогает.
        /// </summary>
        public static bool ConsumeIngredients(Dictionary<IngredientItem, int> required)
        {
            if (!HasAllIngredients(required)) return false;

            foreach (var (item, amount) in required)
                Remove(item, amount);

            return true;
        }

        /// <summary>
        /// Возвращает список ингредиентов, которых не хватает.
        /// </summary>
        public static List<IngredientItem> GetMissingIngredients(IEnumerable<IngredientItem> required)
        {
            var missing = new List<IngredientItem>();
            if (required == null) return missing;

            foreach (var item in required)
                if (GetAmount(item) <= 0) missing.Add(item);

            return missing;
        }

        public static Dictionary<IngredientItem, int> GetAllIngredients()
    => new(_ingredientInventory);

        public static Dictionary<FoodItem, int> GetAllFood()
    => new(_foodInventory);

        /// <summary>
        /// Возвращает детальный отчёт о нехватающих ингредиентах: (есть, нужно).
        /// </summary>
        public static Dictionary<IngredientItem, (int have, int need)> GetMissingIngredientsDetailed(Dictionary<IngredientItem, int> required)
        {
            var missing = new Dictionary<IngredientItem, (int have, int need)>();
            if (required == null) return missing;

            foreach (var (item, need) in required)
            {
                int have = GetAmount(item);
                if (have < need) missing[item] = (have, need);
            }
            return missing;
        }

        // ==================== Приватные хелперы ====================

        private static bool ValidateInput<T>(T item, int amount, string action) where T : class
        {
            if (item == null)
            {
                Debug.LogWarning($"[Inventory] Попытка {action} null-объект");
                return false;
            }
            if (amount <= 0)
            {
                Debug.LogWarning($"[Inventory] Количество должно быть > 0, получено: {amount}");
                return false;
            }
            return true;
        }
    }
}