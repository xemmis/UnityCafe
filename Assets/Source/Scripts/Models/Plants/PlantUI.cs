using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Models.Plant
{
    public sealed class PlantUI : MonoBehaviour
    {
        [SerializeField] private Button _btn = null;
        [SerializeField] private Transform _plantPos = null;
        [SerializeField] private PlantData _plantData;
        private GardenUI _gardenUI = null;

        private void Awake()
        {
            if (_gardenUI == null)
            {
                _gardenUI = GetComponentInParent<GardenUI>();
            }

            if (_btn == null)
            {
                _btn = GetComponentInChildren<Button>();
            }
        }

        public void HandleClick()
        {
            if (_gardenUI == null)
                return;

            _gardenUI.SpawnSpecificPlant(_plantData, _plantPos);
        }
    }
}