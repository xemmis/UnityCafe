using Models;
using Specs;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public sealed class CraftingPlateVisualizer : MonoBehaviour, ICraftingVisualizer
    {
        [SerializeField] private GameObject _popap = null;
        private List<IUICell<IngredientItem>> _ingredientCells = new();
        private Animator _animator = null;
        public static CraftingPlateVisualizer Instance = null;

        private void Awake()
        {
            TryGetComponent<Animator>(out _animator);
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

        public void Clear()
        {
            foreach (IUICell<IngredientItem> cell in _ingredientCells)
                cell.SetItem(null);

            _popap.SetActive(false);
        }

        public void Visualize()
        {
            Dictionary<IngredientItem, int> ingredientItems = BakeryInventory.GetAllIngredients();
            _popap.SetActive(true);

            using var enumerator = ingredientItems.GetEnumerator();

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