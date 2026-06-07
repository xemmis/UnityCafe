using Models;
using Specs;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public sealed class CraftingPlateVisualizer : MonoBehaviour, ICraftingVisualizer
    {
        private List<ICraftingCell> _ingredientCells = new();
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

        public void RegisterCell(ICraftingCell cell)
        {
            _ingredientCells.Add(cell);
        }

        public void Clear()
        {
            foreach (ICraftingCell cell in _ingredientCells)
                cell.SetIngredient(null);
        }

        public void Visualize()
        {
            Dictionary<IngredientItem, int> ingredientItems = BakeryInventory.GetAllIngredients();

            using var enumerator = ingredientItems.GetEnumerator();

            foreach (ICraftingCell cell in _ingredientCells)
            {
                if (enumerator.MoveNext())
                    cell.SetIngredient(enumerator.Current.Key);
                else
                    cell.SetIngredient(null);
            }
        }
    }

}