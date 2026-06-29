using Core;
using Core.Food;
using Specs;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Models.Plant
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PlantBehavior : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private float _growDuration = 150f;
        [SerializeField] private bool _canHarvest = false;
        [SerializeField] private PlantUIVisualization _visualization = null;
        [SerializeField] private PlantSO _plantData = null;

        private SpriteRenderer _renderer = null;
        private PlantStage _currentStage = null;
        private Coroutine _wishRoutine = null;
        private IWish _currentWish = null;
        private float _growthSpeed = 1f;
        private int _stagesCount = 0;
        private float _growProgress;

        public bool CanHarvest => _canHarvest;
        public PlantStage PlantStage => _currentStage;
        public PlantSO PlantData => _plantData;

        public void SetGrowthSpeed(float speed) => _growthSpeed = Mathf.Max(0f, speed);
        public void PauseGrowth() => _growthSpeed = 0f;
        public void ResumeGrowth() => _growthSpeed = 1f;
        public void BoostGrowth(float multiplier) => _growthSpeed = multiplier;

        private void Start()
        {
            if (_plantData != null)
                Initialize(_plantData);
            else
                Debug.LogError($"[PlantBehavior] No plant data assigned on {gameObject.name}!");
        }

        public void Initialize(PlantSO plant)
        {
            _plantData = plant;

            if (_renderer == null)
                _renderer = GetComponent<SpriteRenderer>();

            _renderer.color = Color.white;
            _stagesCount = 0;
            _growProgress = 0f;
            _canHarvest = false;
            _growthSpeed = 1f;

            HandleGrow();
            RestartWishRoutine();
        }

        public void AddGrowthProgress(float seconds)
        {
            _growProgress = Mathf.Min(_growProgress + seconds, _growDuration);

            if (_growProgress >= _growDuration && !_canHarvest)
                HandleGrow();

            // Сбрасываем текущее желание и перезапускаем цикл
            _currentWish = null;
            _visualization?.OnWishFulfilled();
            RestartWishRoutine();
        }

        private void HandleGrow()
        {
            if (_stagesCount >= _plantData.Stages.Count)
            {
                _canHarvest = true;

                if (_plantData.ReadyForHarvestSpr != null)
                    _renderer.sprite = _plantData.ReadyForHarvestSpr;

                // Останавливаем желания — растение готово к сбору
                StopWishRoutine();

                _visualization?.InitializeProgress(this);
                return;
            }

            _currentStage = _plantData.Stages[_stagesCount];

            if (_currentStage != null)
            {
                if (_currentStage.StageSprite != null)
                    _renderer.sprite = _currentStage.StageSprite;

                _growDuration = _currentStage.GrowTimer;
                _growProgress = 0f;
                _stagesCount++;

                _visualization?.InitializeProgress(this);
            }
            else
            {
                Debug.LogError($"[PlantBehavior] Stage is null at index {_stagesCount}!");
            }
        }

        private void FixedUpdate()
        {
            if (_plantData != null && !_canHarvest)
                TimerCheck();
        }

        private void TimerCheck()
        {
            if (_canHarvest || _growthSpeed <= 0f || _growDuration <= 0f) return;

            _growProgress += Time.fixedDeltaTime * _growthSpeed;

            if (_growProgress >= _growDuration)
            {
                HandleGrow();
                _growProgress = 0f;
            }
        }

        // ──────────────────────────────────────────
        // Цикл желаний
        // ──────────────────────────────────────────

        private void RestartWishRoutine()
        {
            StopWishRoutine();
            // Не запускаем если уже готово к сбору
            if (!_canHarvest)
                _wishRoutine = StartCoroutine(WishCycleRoutine());
        }

        private void StopWishRoutine()
        {
            if (_wishRoutine != null)
            {
                StopCoroutine(_wishRoutine);
                _wishRoutine = null;
            }
        }

        /// <summary>
        /// Бесконечный цикл желаний пока растение не выросло.
        /// </summary>
        private IEnumerator WishCycleRoutine()
        {
            while (!_canHarvest)
            {
                float waitTime = _growDuration > 0f
                    ? UnityEngine.Random.Range(_growDuration / 4f, _growDuration / 2f)
                    : 5f;

                yield return new WaitForSeconds(waitTime);

                // Проверяем снова — за время ожидания растение могло вырасти
                if (_canHarvest) yield break;

                // Показываем новое желание только если предыдущее выполнено
                if (_currentWish == null)
                {
                    _currentWish = WishFactory.Instance.CreateRandomWish();
                    _visualization?.VisualizeWish(_currentWish);
                }
            }
        }

        // ──────────────────────────────────────────
        // Сброс / Сбор
        // ──────────────────────────────────────────

        public void ResetGrowth()
        {
            _growProgress = 0f;
            _stagesCount = 0;
            _canHarvest = false;
            _growthSpeed = 1f;
            _currentWish = null;

            if (_plantData != null && _plantData.NotReadyForHarvestSpr != null)
                _renderer.sprite = _plantData.NotReadyForHarvestSpr;

            if (_plantData != null && _plantData.Stages.Count > 0)
                HandleGrow();

            RestartWishRoutine();
        }

        public float GetGrowthProgress() =>
            _growDuration > 0f ? Mathf.Clamp01(_growProgress / _growDuration) : 1f;

        private void OnValidate()
        {
            if (_plantData != null && _plantData.Stages.Count > 0 && _growDuration == 0)
                _growDuration = _plantData.Stages[0].GrowTimer;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_canHarvest)
                HandleHarvest();
        }

        private void HandleHarvest()
        {
            BakeryInventory.Add(_plantData.Ingredinet, _plantData.IngredientOutput);
            _visualization?.OnHarvested();
            ResetGrowth();
        }

        private void OnDestroy()
        {
            StopWishRoutine();
        }
    }
}