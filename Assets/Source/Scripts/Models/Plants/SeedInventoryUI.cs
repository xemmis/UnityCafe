using Core;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models.Plant
{
    public sealed class SeedInventoryUI : MonoBehaviour
    {
        [SerializeField] private Transform _visualizePos = null;
        [SerializeField] private Transform _clearPos = null;
        [SerializeField] private GameObject _uiPopap = null;
        [SerializeField] private bool _openFlag = false;
        [SerializeField] private List<SeedItemCell> _cells = new();

        private void Start()
        {
            SeedInventory.OnSeedInventoryChange += OnSeedChanged;
        }

        private void OnDestroy()
        {
            SeedInventory.OnSeedInventoryChange -= OnSeedChanged;
        }

        private void OnSeedChanged(PlantSO _, int __) => Refresh();

        public void Visualize()
        {
            _openFlag = !_openFlag;
            Refresh();

            if (_openFlag)
                _uiPopap.transform.DOMove(_visualizePos.position, 1);
            else
                _uiPopap.transform.DOMove(_clearPos.position, 1);
        }

        public void Refresh()
        {
            var allSeeds = SeedInventory.GetAllSeeds()
                .Where(kvp => kvp.Value > 0)
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key.name)
                .Take(_cells.Count)
                .ToList();

            for (int i = 0; i < _cells.Count; i++)
            {
                if (i < allSeeds.Count)
                    _cells[i].SetItem(allSeeds[i].Key, allSeeds[i].Value);
                else
                    _cells[i].ClearCell();
            }
        }
    }
}