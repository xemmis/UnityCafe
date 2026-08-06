using DG.Tweening;
using Models.Food;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Food
{
    public sealed class FoodInventoryUI : MonoBehaviour
    {
        [SerializeField] private Transform _foodCellsContainer;
        [SerializeField] private FoodItemCell _foodCellPrefab;
        [SerializeField] private int _maxDisplayedItems = 24;
        [SerializeField] private Transform _visualizePos = null;
        [SerializeField] private Transform _clearPos = null;
        [SerializeField] private GameObject _uiPopap = null;
        [SerializeField] private bool _openFlag = false;
        [SerializeField] private List<FoodItemCell> _cells = new();

        private void Start()
        {
            BakeryInventory.OnFoodInventoryChange += OnFoodChanged;
            GameTimeManager.Instance.OnDayConditionChange.AddListener(HandleEndDayVisual);
        }

        private void HandleEndDayVisual(bool condition)
        {
            if (!condition && _openFlag)
                Visualize();
        }

        public void Visualize()
        {
            _openFlag = !_openFlag;
            Refresh();

            if (_openFlag)
                _uiPopap.transform.DOMove(_visualizePos.position, 1);
            else
                _uiPopap.transform.DOMove(_clearPos.position, 1);
        }

        public void ClearUI()
        {
            _openFlag = false;

            _uiPopap.transform.DOMove(_clearPos.position, 1);
        }

        private void OnDestroy()
        {
            BakeryInventory.OnFoodInventoryChange -= OnFoodChanged;
            GameTimeManager.Instance.OnDayConditionChange.RemoveListener(HandleEndDayVisual);
        }

        private void OnFoodChanged(FoodItem _, int __) => Refresh();

        public void Refresh()
        {
            var allFood = BakeryInventory.GetAllFood()
                .Where(kvp => kvp.Value > 0)
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key.FoodName)
                .Take(_maxDisplayedItems)
                .ToList();

            // EnsureCells(allFood.Count);

            for (int i = 0; i < _cells.Count; i++)
            {
                if (i < allFood.Count)
                    _cells[i].SetItem(allFood[i].Key);
                else
                    _cells[i].ClearCell();
            }
        }

        /* Forbidden method
        private void EnsureCells(int requiredCount)
        {
            while (_cells.Count < requiredCount && _cells.Count < _maxDisplayedItems)
            {
                var cell = Instantiate(_foodCellPrefab, _foodCellsContainer);
                _cells.Add(cell);
            }
        }
        */
    }
}