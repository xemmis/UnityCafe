using Models.Plant;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public sealed class GardenManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Transform _plantsParent;

        [Header("Debug")]
        [SerializeField] private bool _spawnOnStart = false;
        [SerializeField] private int _spawnCountOnStart = 3;

        // Словарь для отслеживания активных растений и их точек спавна
        private readonly Dictionary<Transform, Plant> _activePlants = new();
        private readonly List<Transform> _availableSpawnPoints = new();

        public IReadOnlyDictionary<Transform, Plant> ActivePlants => _activePlants;
        public int ActivePlantCount => _activePlants.Count;

        // События
        public event System.Action<Plant, Transform> OnPlantSpawned;
        public event System.Action<Plant, Transform> OnPlantDespawned;
        public event System.Action<Plant> OnPlantFullyGrown;

        private void Awake()
        {
            // Инициализируем список доступных точек спавна
            if (_spawnPoints != null)
            {
                _availableSpawnPoints.AddRange(_spawnPoints);
            }

            if (_plantsParent == null)
            {
                _plantsParent = transform;
            }
        }

        private void Start()
        {
            if (_spawnOnStart)
            {
                SpawnMultipleRandom(_spawnCountOnStart);
            }
        }

        /// <summary>
        /// Спавнит растение в указанной точке с конкретными настройками
        /// </summary>
        public Plant SpawnPlant(PlantData plantData, Transform spawnPoint, PlantSO plantSO = null)
        {
            if (spawnPoint == null)
            {
                Debug.LogError("[GardenManager] Spawn point is null!");
                return null;
            }

            if (_activePlants.ContainsKey(spawnPoint))
            {
                Debug.LogWarning($"[GardenManager] Spawn point {spawnPoint.name} is already occupied!");
                return null;
            }

            Plant plant = PlantFabric.Instance.Spawn(plantData, spawnPoint.position, plantSO);

            if (plant != null)
            {
                plant.transform.SetParent(_plantsParent);
                _activePlants[spawnPoint] = plant;
                _availableSpawnPoints.Remove(spawnPoint);

                // Подписываемся на события растения
                plant.OnPlantFullyGrown += HandlePlantFullyGrown;

                OnPlantSpawned?.Invoke(plant, spawnPoint);

                Debug.Log($"[GardenManager] Plant spawned at {spawnPoint.name}");
            }

            return plant;
        }

        /// <summary>
        /// Спавнит растение в первой свободной точке
        /// </summary>
        public Plant SpawnPlantAtFirstAvailable(PlantData plantData, PlantSO plantSO = null)
        {
            Transform freePoint = GetFreeSpawnPoint();

            if (freePoint == null)
            {
                Debug.LogWarning("[GardenManager] No free spawn points available!");
                return null;
            }

            return SpawnPlant(plantData, freePoint, plantSO);
        }

        /// <summary>
        /// Спавнит случайное растение в указанной точке
        /// </summary>
        public Plant SpawnRandomPlant(Transform spawnPoint)
        {
            if (spawnPoint == null)
            {
                Debug.LogError("[GardenManager] Spawn point is null!");
                return null;
            }

            return SpawnPlant(GetRandomPlantData(), spawnPoint);
        }

        /// <summary>
        /// Спавнит случайное растение в первой свободной точке
        /// </summary>
        public Plant SpawnRandomPlantAtFirstAvailable()
        {
            Transform freePoint = GetFreeSpawnPoint();

            if (freePoint == null)
            {
                Debug.LogWarning("[GardenManager] No free spawn points available!");
                return null;
            }

            return SpawnRandomPlant(freePoint);
        }

        /// <summary>
        /// Спавнит несколько случайных растений в свободных точках
        /// </summary>
        public void SpawnMultipleRandom(int count)
        {
            int spawnedCount = 0;

            for (int i = 0; i < count; i++)
            {
                if (SpawnRandomPlantAtFirstAvailable() != null)
                {
                    spawnedCount++;
                }
                else
                {
                    break; // Нет больше свободных точек
                }
            }

            Debug.Log($"[GardenManager] Spawned {spawnedCount}/{count} random plants");
        }

        /// <summary>
        /// Спавнит конкретный набор растений
        /// </summary>
        public void SpawnSpecificPlants(List<PlantSpawnRequest> requests)
        {
            foreach (var request in requests)
            {
                if (request.UseFirstAvailable)
                {
                    SpawnPlantAtFirstAvailable(request.PlantData, request.PlantSO);
                }
                else
                {
                    SpawnPlant(request.PlantData, request.SpawnPoint, request.PlantSO);
                }
            }
        }

        /// <summary>
        /// Убирает растение с указанной точки
        /// </summary>
        public bool DespawnPlantAtPoint(Transform spawnPoint)
        {
            if (!_activePlants.TryGetValue(spawnPoint, out Plant plant))
            {
                Debug.LogWarning($"[GardenManager] No plant at spawn point {spawnPoint.name}");
                return false;
            }

            // Отписываемся от событий
            plant.OnPlantFullyGrown -= HandlePlantFullyGrown;

            // Возвращаем в пул
            PlantFabric.Instance.Despawn(plant);

            _activePlants.Remove(spawnPoint);
            _availableSpawnPoints.Add(spawnPoint);

            OnPlantDespawned?.Invoke(plant, spawnPoint);

            Debug.Log($"[GardenManager] Plant despawned from {spawnPoint.name}");
            return true;
        }

        /// <summary>
        /// Убирает все растения
        /// </summary>
        public void ClearAllPlants()
        {
            var spawnPoints = new List<Transform>(_activePlants.Keys);

            foreach (var point in spawnPoints)
            {
                DespawnPlantAtPoint(point);
            }

            Debug.Log($"[GardenManager] All plants cleared");
        }

        /// <summary>
        /// Собирает готовое растение (деспавнит и возвращает награду)
        /// </summary>
        public bool HarvestPlant(Transform spawnPoint)
        {
            if (!_activePlants.TryGetValue(spawnPoint, out Plant plant))
            {
                Debug.LogWarning($"[GardenManager] No plant to harvest at {spawnPoint.name}");
                return false;
            }

            if (!plant.IsFullyGrown)
            {
                Debug.LogWarning($"[GardenManager] Plant at {spawnPoint.name} is not fully grown yet!");
                return false;
            }

            // Здесь можно добавить логику награды
            if (plant.PlantSO != null)
            {
                //   Debug.Log($"[GardenManager] Harvested plant! Rewards: {plant.PlantSO.ExperienceReward} XP, {plant.PlantSO.CoinsReward} Coins");
                // GameManager.Instance.AddExperience(plant.PlantSO.ExperienceReward);
                // GameManager.Instance.AddCoins(plant.PlantSO.CoinsReward);
            }

            return DespawnPlantAtPoint(spawnPoint);
        }

        /// <summary>
        /// Получает свободную точку спавна
        /// </summary>
        public Transform GetFreeSpawnPoint()
        {
            if (_availableSpawnPoints.Count == 0)
            {
                // Пробуем найти среди всех точек
                foreach (var point in _spawnPoints)
                {
                    if (!_activePlants.ContainsKey(point))
                    {
                        _availableSpawnPoints.Add(point);
                        return point;
                    }
                }

                return null;
            }

            return _availableSpawnPoints[0];
        }

        /// <summary>
        /// Проверяет, есть ли свободные точки спавна
        /// </summary>
        public bool HasFreeSpawnPoints()
        {
            return GetFreeSpawnPoint() != null;
        }

        /// <summary>
        /// Получает случайные данные растения из доступных
        /// </summary>
        private PlantData GetRandomPlantData()
        {
            var availableTypes = PlantFabric.Instance.GetAvailablePlantTypes();

            if (availableTypes.Count == 0)
            {
                Debug.LogError("[GardenManager] No plant types available!");
                return default;
            }

            return availableTypes[Random.Range(0, availableTypes.Count)];
        }

        private void HandlePlantFullyGrown(Plant plant)
        {
            OnPlantFullyGrown?.Invoke(plant);

            // Можно добавить визуальный эффект или уведомление
            Debug.Log($"[GardenManager] Plant is ready to harvest!");
        }

        private void OnDestroy()
        {
            ClearAllPlants();
        }

#if UNITY_EDITOR
        // Кнопки для тестирования в инспекторе
        [ContextMenu("Spawn Random Plant")]
        private void EditorSpawnRandomPlant()
        {
            SpawnRandomPlantAtFirstAvailable();
        }

        [ContextMenu("Spawn 3 Random Plants")]
        private void EditorSpawnMultiplePlants()
        {
            SpawnMultipleRandom(3);
        }

        [ContextMenu("Clear All Plants")]
        private void EditorClearAllPlants()
        {
            ClearAllPlants();
        }
#endif
    }
}
