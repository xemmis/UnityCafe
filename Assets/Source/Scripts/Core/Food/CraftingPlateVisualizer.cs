using DG.Tweening;
using Models.Food;
using Specs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Food
{
    public sealed class CraftingPlateVisualizer : MonoBehaviour, ICraftingVisualizer
    {
        [SerializeField] private GameObject _popap = null;
        [SerializeField] private Transform _visualizePos = null;
        [SerializeField] private Transform _clearPos = null;
        private List<IUICell<IngredientItem>> _ingredientCells = new();
        public static CraftingPlateVisualizer Instance = null;
        private Coroutine _clearItemRoutine = null;
        [SerializeField]  private bool _openFlag = false;

        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (CraftingPlateVisualizer.Instance == null)
            {
                CraftingPlateVisualizer.Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterCell(IUICell<IngredientItem> cell)
        {
            _ingredientCells.Add(cell);
        }

        private IEnumerator ClearItemsInCellTick()
        {
            yield return new WaitForSeconds(1);
            foreach (IUICell<IngredientItem> cell in _ingredientCells)
                cell.SetItem(null);
            _clearItemRoutine = null;
        }

        public void Clear()
        {
            if (!_openFlag) return; // уже закрыто — не делаем ничего

            _openFlag = false;
            _popap.transform.DOMove(_clearPos.position, 1f);
            _clearItemRoutine = StartCoroutine(ClearItemsInCellTick());
        }

        public void Visualize()
        {
            if (_clearItemRoutine != null)
            {
                StopCoroutine(_clearItemRoutine);
                _clearItemRoutine = null;
            }

            // Вместо toggle — явно открываем или закрываем
            if (!_openFlag)
            {
                Open();
            }
            else
            {
                Clear();
            }
        }

        private void Open()
        {
            _openFlag = true;
            Dictionary<IngredientItem, int> ingredientItems = BakeryInventory.GetAllIngredients();
            _popap.transform.DOMove(_visualizePos.position, 1);
             var enumerator = ingredientItems.GetEnumerator();

            foreach (IUICell<IngredientItem> cell in _ingredientCells)
            {
                if (enumerator.MoveNext())
                    cell.SetItem(enumerator.Current.Key);
                else
                    cell.SetItem(null);
            }
        }
    }

}