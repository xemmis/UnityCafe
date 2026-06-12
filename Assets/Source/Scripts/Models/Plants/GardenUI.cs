using Core;
using UnityEngine;

namespace Models.Plant
{
    public sealed class GardenUI : MonoBehaviour
    {
        [SerializeField] private Transform _specificSpawnPoint;
        [SerializeField] private GardenManager _gardenManager;

        public void SpawnRandomPlant()
        {
            _gardenManager.SpawnRandomPlantAtFirstAvailable();
        }

        public void SpawnSpecificPlant(PlantData plantData, Transform plantPos = null)
        {
            if (plantPos != null)
            {
                _gardenManager.SpawnPlant(plantData, plantPos);
            }
            else if (_specificSpawnPoint != null)
            {
                _gardenManager.SpawnPlant(plantData, _specificSpawnPoint);
            }
        }

        public void HarvestAllReadyPlants()
        {
            foreach (var kvp in _gardenManager.ActivePlants)
            {
                if (kvp.Value.IsFullyGrown)
                {
                    _gardenManager.HarvestPlant(kvp.Key);
                }
            }
        }

        public void ClearAllPlants()
        {
            _gardenManager.ClearAllPlants();
        }

        public void OnGardenButtonClicked()
        {
            SpawnRandomPlant();
        }
    }
}