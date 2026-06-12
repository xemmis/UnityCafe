namespace Core
{
    using Models.Plant;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Utils;

    public sealed class PlantFabric : MonoBehaviour
    {
        [SerializeField] private List<PlantData> _plantDatas = new();
        [SerializeField] private int _initialPoolSize = 5;
        [SerializeField] private int _maxPoolSize = 20;

        private readonly Dictionary<PlantData, ObjectPool<Plant>> _pools = new();
        public static PlantFabric Instance { get; private set; } = null;

        private void Awake()
        {
            if (PlantFabric.Instance == null)
            {
                PlantFabric.Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializePools();
        }

        private void InitializePools()
        {
            foreach (PlantData plantData in _plantDatas)
            {
                if (!plantData.IsValid)
                {
                    Debug.LogWarning($"[PlantFabric] PlantData '{plantData.PlantName}' has no prefab — skipping.");
                    continue;
                }

                Plant plantComponent = plantData.Prefab.GetComponent<Plant>();

                if (plantComponent == null)
                {
                    Debug.LogError($"[PlantFabric] Prefab '{plantData.Prefab.name}' missing Plant component — skipping.");
                    continue;
                }

                var pool = new ObjectPool<Plant>(plantComponent, _initialPoolSize, _maxPoolSize, transform);
                _pools[plantData] = pool;

                Debug.Log($"[PlantFabric] Pool initialized for: {plantData}");
            }
        }

        /// <summary>
        /// Спавнит растение с указанными данными и настройками
        /// </summary>
        /// <param name="plantData">Данные растения для спавна</param>
        /// <param name="spawnPosition">Позиция спавна</param>
        /// <param name="plantSO">ScriptableObject с настройками растения</param>
        /// <returns>Заспавненное и инициализированное растение</returns>
        public Plant Spawn(PlantData plantData, Vector3 spawnPosition, PlantSO plantSO = null)
        {
            if (!_pools.TryGetValue(plantData, out ObjectPool<Plant> pool))
            {
                Debug.LogError($"[PlantFabric] No pool for PlantData '{plantData.PlantName}'. Make sure it's added to the plantDatas list.");
                return null;
            }

            Plant plant = pool.Get();

            // Устанавливаем позицию и поворот
            plant.transform.position = spawnPosition;
            plant.transform.rotation = Quaternion.identity;

            // Инициализируем растение
            PlantSO soToUse = plantSO ?? plantData.DefaultPlantSO;
            if (soToUse == null)
            {
                Debug.LogError($"[PlantFabric] No PlantSO provided for {plantData.PlantName} and no default configured!");
                pool.Return(plant);
                return null;
            }

            plant.Initialize(soToUse, plantData);

            // Настраиваем визуализацию если есть
            SetupVisuals(plant, plantData);

            Debug.Log($"[PlantFabric] Spawned plant: {plantData.PlantName} at {spawnPosition}");

            return plant;
        }

        /// <summary>
        /// Спавнит растение по умолчанию (без дополнительных настроек)
        /// </summary>
        public Plant SpawnDefault(PlantData plantData, Vector3 spawnPosition)
        {
            return Spawn(plantData, spawnPosition, plantData.DefaultPlantSO);
        }

        /// <summary>
        /// Спавнит случайное растение из доступных
        /// </summary>
        public Plant SpawnRandom(Vector3 spawnPosition)
        {
            if (_plantDatas.Count == 0)
            {
                Debug.LogError("[PlantFabric] No plant data available for random spawn.");
                return null;
            }

            PlantData randomPlantData = _plantDatas[UnityEngine.Random.Range(0, _plantDatas.Count)];
            return Spawn(randomPlantData, spawnPosition);
        }

        /// <summary>
        /// Возвращает растение в пул
        /// </summary>
        public void Despawn(Plant plant)
        {
            if (plant == null)
            {
                Debug.LogError("[PlantFabric] Cannot despawn null plant.");
                return;
            }

            PlantData plantData = plant.PlantData;

            if (!_pools.TryGetValue(plantData, out ObjectPool<Plant> pool))
            {
                Debug.LogError($"[PlantFabric] No pool for PlantData '{plantData.PlantName}'. Destroying plant instead.");
                Destroy(plant.gameObject);
                return;
            }

            // Очищаем состояние растения перед возвратом в пул
            plant.ResetState();
            pool.Return(plant);

            Debug.Log($"[PlantFabric] Despawned plant: {plantData.PlantName}");
        }

        /// <summary>
        /// Возвращает растение в пул с явным указанием PlantData
        /// </summary>
        public void Despawn(PlantData plantData, Plant plant)
        {
            if (plant == null)
            {
                Debug.LogError("[PlantFabric] Cannot despawn null plant.");
                return;
            }

            if (!_pools.TryGetValue(plantData, out ObjectPool<Plant> pool))
            {
                Debug.LogError($"[PlantFabric] No pool for PlantData '{plantData.PlantName}'. Destroying plant instead.");
                Destroy(plant.gameObject);
                return;
            }

            plant.ResetState();
            pool.Return(plant);

            Debug.Log($"[PlantFabric] Despawned plant: {plantData.PlantName}");
        }

        private void SetupVisuals(Plant plant, PlantData plantData)
        {
            if (plantData.HasVisualOverride)
            {
                if (plantData.OverrideMaterial != null)
                {
                    var renderer = plant.GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = plantData.OverrideMaterial;
                    }
                }

                if (plantData.CustomScale != Vector3.one)
                {
                    plant.transform.localScale = plantData.CustomScale;
                }
            }
        }

        /// <summary>
        /// Проверяет, есть ли пул для указанного типа растения
        /// </summary>
        public bool HasPool(PlantData plantData)
        {
            return _pools.ContainsKey(plantData);
        }

        /// <summary>
        /// Получает количество доступных растений в пуле
        /// </summary>
        public int GetAvailableCount(PlantData plantData)
        {
            return _pools.TryGetValue(plantData, out ObjectPool<Plant> pool) ? pool.CountInactive : 0;
        }

        /// <summary>
        /// Получает все доступные PlantData
        /// </summary>
        public IReadOnlyList<PlantData> GetAvailablePlantTypes()
        {
            return _plantDatas.AsReadOnly();
        }

        /// <summary>
        /// Предварительный прогрев пулов
        /// </summary>
        public void PreWarmPools()
        {
            foreach (var kvp in _pools)
            {
                int currentCount = kvp.Value.CountInactive;
                int needed = _initialPoolSize - currentCount;

                for (int i = 0; i < needed; i++)
                {
                    Plant plant = kvp.Value.Get();
                    if (plant != null)
                    {
                        plant.ResetState();
                        kvp.Value.Return(plant);
                    }
                }

                Debug.Log($"[PlantFabric] Pre-warmed pool for {kvp.Key.PlantName}: {kvp.Value.CountInactive} available");
            }
        }

        private void OnDestroy()
        {
            foreach (var pool in _pools.Values)
                pool.Dispose();

            _pools.Clear();
        }
    }
}
