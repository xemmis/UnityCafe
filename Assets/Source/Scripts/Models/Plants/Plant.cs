using Core;
using Specs;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Models.Plant
{
    public sealed class Plant : MonoBehaviour
    {
        [SerializeField] private IPlantUIVisualizer _visualizer = null;

        private PlantWishType _currentWish = PlantWishType.None;
        private PlantSO _plantSO = null;
        private PlantData _plantData;
        private Coroutine _wishCoroutine;
        private bool _hasActiveWish = false;
        private bool _isInitialized = false;

        // Система роста
        private float _currentGrowthTime;
        private int _wishesCompleted = 0;
        private bool _isFullyGrown = false;

        public PlantSO PlantSO => _plantSO;
        public PlantData PlantData => _plantData;
        public bool HasActiveWish => _hasActiveWish;
        public PlantWishType CurrentWish => _currentWish;
        public bool IsFullyGrown => _isFullyGrown;

        private void Awake()
        {
            if (_visualizer == null)
            {
                _visualizer = GetComponentInChildren<IPlantUIVisualizer>();
            }
        }

        /// <summary>
        /// Инициализация растения через фабрику
        /// </summary>
        public void Initialize(PlantSO plantSO, PlantData plantData)
        {
            _plantSO = plantSO ?? throw new ArgumentNullException(nameof(plantSO));
            _plantData = plantData; // Структура, не может быть null

            if (_plantSO != null)
            {
                _currentGrowthTime = _plantSO.BaseGrowthTime;
            }

            _isInitialized = true;
            StartWishCycle();
        }

        private void Start()
        {
            // Если растение не было инициализировано через фабрику, 
            // запускаем с настройками по умолчанию
            if (!_isInitialized && _plantSO != null)
            {
                // Создаем дефолтный PlantData если не был передан
                PlantData defaultPlantData = new PlantData();
                Initialize(_plantSO, defaultPlantData);
            }
        }

        private void StartWishCycle()
        {
            if (_wishCoroutine != null)
            {
                StopCoroutine(_wishCoroutine);
            }
            _wishCoroutine = StartCoroutine(WishCycleRoutine());
        }

        private IEnumerator WishCycleRoutine()
        {
            // Небольшая начальная задержка
            yield return new WaitForSeconds(Random.Range(2f, 5f));

            while (!_isFullyGrown)
            {
                // Ждем случайное время перед появлением нового желания
                float minWaitTime = _plantSO != null ? _plantSO.MinWishInterval : 5f;
                float maxWaitTime = _plantSO != null ? _plantSO.MaxWishInterval : 15f;
                float waitTime = Random.Range(minWaitTime, maxWaitTime);

                yield return new WaitForSeconds(waitTime);

                if (!_hasActiveWish && !_isFullyGrown)
                {
                    GenerateNewWish();
                }
            }
        }

        private void GenerateNewWish()
        {
            // Используем веса из PlantSO если доступны
            PlantWishType newWish;

            if (_plantSO != null && _plantSO.WishWeights != null && _plantSO.WishWeights.Length > 0)
            {
                newWish = GetWeightedRandomWish();
            }
            else
            {
                // Равномерное распределение
                var wishTypes = Enum.GetValues(typeof(PlantWishType));
                do
                {
                    newWish = (PlantWishType)wishTypes.GetValue(Random.Range(1, wishTypes.Length));
                } while (newWish == PlantWishType.None);
            }

            SetNewWish(newWish);
        }

        private PlantWishType GetWeightedRandomWish()
        {
            float totalWeight = 0f;
            foreach (var wishWeight in _plantSO.WishWeights)
            {
                totalWeight += wishWeight.Weight;
            }

            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (var wishWeight in _plantSO.WishWeights)
            {
                currentWeight += wishWeight.Weight;
                if (randomValue <= currentWeight)
                {
                    return wishWeight.WishType;
                }
            }

            return PlantWishType.Food; // Fallback
        }

        private void SetNewWish(PlantWishType wishType)
        {
            _currentWish = wishType;
            _hasActiveWish = true;
            _visualizer?.VisualizeWish(wishType);

            OnWishGenerated?.Invoke(this, wishType);

            Debug.Log($"Plant {gameObject.name} generated wish: {wishType}");
        }

        public bool HandleWish(PlantWishType plantWishType)
        {
            if (!_hasActiveWish || _currentWish != plantWishType)
            {
                Debug.Log($"Wrong wish for {gameObject.name}! Expected: {_currentWish}, Got: {plantWishType}");
                return false;
            }

            CompleteWish();
            return true;
        }

        private void CompleteWish()
        {
            _hasActiveWish = false;
            _currentWish = PlantWishType.None;
            _wishesCompleted++;

            // Уменьшаем время роста
            if (_plantSO != null)
            {
                float growthReduction = _currentGrowthTime * _plantSO.GrowthReductionPerWish;
                _currentGrowthTime -= growthReduction;
                _currentGrowthTime = Mathf.Max(_currentGrowthTime, _plantSO.MinGrowthTime);

                Debug.Log($"Wish completed for {gameObject.name}! Growth time: {_currentGrowthTime:F1}s. Progress: {_wishesCompleted}/{_plantSO.WishesToGrow}");
            }

            // Очищаем UI
            _visualizer?.ClearUI();

            OnWishCompleted?.Invoke(this);

            // Проверяем, выросло ли растение
            CheckGrowth();
        }

        private void CheckGrowth()
        {
            if (_plantSO != null && _wishesCompleted >= _plantSO.WishesToGrow)
            {
                GrowPlant();
            }
        }

        private void GrowPlant()
        {
            _isFullyGrown = true;
            _visualizer?.VisualizeReadyCondition();

            if (_wishCoroutine != null)
            {
                StopCoroutine(_wishCoroutine);
                _wishCoroutine = null;
            }

            Debug.Log($"Plant {gameObject.name} is fully grown!");

            // Событие о полном росте
            OnPlantFullyGrown?.Invoke(this);
        }

        /// <summary>
        /// Сброс состояния растения (для возврата в пул)
        /// </summary>
        public void ResetState()
        {
            if (_wishCoroutine != null)
            {
                StopCoroutine(_wishCoroutine);
                _wishCoroutine = null;
            }

            _currentWish = PlantWishType.None;
            _hasActiveWish = false;
            _wishesCompleted = 0;
            _isFullyGrown = false;
            _isInitialized = false;
            _plantSO = null;
            _plantData = default; // Сбрасываем структуру в дефолтное состояние

            if (_plantSO != null)
            {
                _currentGrowthTime = _plantSO.BaseGrowthTime;
            }
            else
            {
                _currentGrowthTime = 0f;
            }

            _visualizer?.ClearUI();

            // Отписываем все события
            OnPlantFullyGrown = null;
            OnWishGenerated = null;
            OnWishCompleted = null;
        }

        public float GetGrowthProgress()
        {
            if (_plantSO == null) return 0f;
            return Mathf.Clamp01((float)_wishesCompleted / _plantSO.WishesToGrow);
        }

        // События
        public event Action<Plant> OnPlantFullyGrown;
        public event Action<Plant, PlantWishType> OnWishGenerated;
        public event Action<Plant> OnWishCompleted;

        private void OnDestroy()
        {
            if (_wishCoroutine != null)
            {
                StopCoroutine(_wishCoroutine);
            }

            // Очищаем события при уничтожении
            OnPlantFullyGrown = null;
            OnWishGenerated = null;
            OnWishCompleted = null;
        }
    }
}