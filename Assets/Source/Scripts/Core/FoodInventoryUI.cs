using Models;
using Specs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Core
{
    public class FoodInventoryUI : MonoBehaviour
    {
        [Header("Food Display")]
        [SerializeField] private Transform _foodCellsContainer;
        [SerializeField] private FoodItemCell _foodCellPrefab;

        [Header("Settings")]
        [SerializeField] private int _maxDisplayedItems = 24;

        private List<IUICell<FoodItemCell>> _foodCells = new();

        private void OnEnable()
        {
            // Подписываемся на события изменения инвентаря еды
            BakeryInventory.OnFoodInventoryChange += OnFoodChanged;

            RefreshFoodDisplay();
        }

        private void OnDisable()
        {
            // Отписываемся от событий
            BakeryInventory.OnFoodInventoryChange -= OnFoodChanged;
        }

        public void RegisterCell(IUICell<FoodItemCell> uICell)
        {
            _foodCells.Add(uICell);
        }


        private void OnFoodChanged(FoodItem item, int newAmount)
        {
            RefreshFoodDisplay();
        }

        private void RefreshFoodDisplay()
        {
            if (_foodCells == null || _foodCells.Count == 0) return;

            var allFood = BakeryInventory.GetAllFood()
                .Where(kvp => kvp.Value > 0) // Только те, что есть в наличии
                .OrderByDescending(kvp => kvp.Value) // Сортируем по количеству
                .ThenBy(kvp => kvp.Key.FoodName)     // Потом по имени
                .ToList();

            int cellIndex = 0;

            // Заполняем существующие предметы
            foreach (var foodPair in allFood)
            {
                if (cellIndex >= _foodCells.Count) break;

                _foodCells[cellIndex].SetItem(foodPair.Key);
                cellIndex++;
            }

            // Очищаем оставшиеся ячейки
            for (int i = cellIndex; i < _foodCells.Count; i++)
            {
                _foodCells[i].ClearCell();
            }
        }

        // Публичный метод для ручного обновления
        public void ForceRefresh()
        {
            RefreshFoodDisplay();
        }

        private void OnDestroy()
        {
            BakeryInventory.OnFoodInventoryChange -= OnFoodChanged;
        }
    }
}