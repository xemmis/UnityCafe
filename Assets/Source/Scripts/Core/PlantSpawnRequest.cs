using UnityEngine;

namespace Core
{
    /// <summary>
    /// Запрос на спавн растения
    /// </summary>
    [System.Serializable]
    public class PlantSpawnRequest
    {
        public PlantData PlantData;
        public PlantSO PlantSO;
        public Transform SpawnPoint;
        public bool UseFirstAvailable = false;
    }
}
