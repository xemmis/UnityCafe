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
            Debug.Log($"[PlantBehavior] Start on {gameObject.name}");
            if (_plantData != null)
            {
                Debug.Log($"[PlantBehavior] Initializing with plant data: {_plantData.name}, Stages count: {_plantData.Stages.Count}");
                Initialize(_plantData);
            }
            else
            {
                Debug.LogError($"[PlantBehavior] No plant data assigned on {gameObject.name}!");
            }
        }

        public void Initialize(PlantSO plant)
        {
            _plantData = plant;

            if (_renderer == null)
            {
                _renderer = GetComponent<SpriteRenderer>();
                Debug.Log($"[PlantBehavior] SpriteRenderer found: {_renderer != null}");
            }

            _renderer.color = Color.white;
            // Сбрасываем счетчик стадий при инициализации
            _stagesCount = 0;
            _growProgress = 0f;
            _canHarvest = false;
            _growthSpeed = 1f;

            Debug.Log($"[PlantBehavior] Initialize - Setting first stage, _stagesCount: {_stagesCount}");
            HandleGrow();

            if (_wishRoutine != null)
            {
                StopCoroutine(_wishRoutine);
            }
            _wishRoutine = StartCoroutine(WishEnableTick());
        }

        public void AddGrowthProgress(float seconds)
        {
            Debug.Log($"[PlantBehavior] AddGrowthProgress - adding {seconds} seconds");
            _growProgress = Mathf.Min(_growProgress + seconds, _growDuration);

            if (_growProgress >= _growDuration && !_canHarvest)
            {
                Debug.Log($"[PlantBehavior] Growth progress reached duration, calling HandleGrow");
                HandleGrow();
            }

            if (_wishRoutine != null)
            {
                StopCoroutine(_wishRoutine);
            }
            _wishRoutine = StartCoroutine(WishEnableTick());
            _visualization?.OnWishFulfilled();
            _currentWish = null;
        }

        private void HandleGrow()
        {
            Debug.Log($"[PlantBehavior] HandleGrow - Current stagesCount: {_stagesCount}, Total stages: {_plantData.Stages.Count}");

            // Проверяем, не достигли ли мы конца стадий
            if (_stagesCount >= _plantData.Stages.Count)
            {
                Debug.Log($"[PlantBehavior] All stages completed! Plant can harvest now.");
                _canHarvest = true;

                // Показываем спрайт готового к сбору растения
                if (_plantData.ReadyForHarvestSpr != null)
                {
                    _renderer.sprite = _plantData.ReadyForHarvestSpr;
                }

                // Уведомляем визуализацию
                _visualization?.InitializeProgress(this);
                return;
            }

            // Получаем текущую стадию
            _currentStage = _plantData.Stages[_stagesCount];

            if (_currentStage != null)
            {
                Debug.Log($"[PlantBehavior] Setting stage {_stagesCount}: {_currentStage.StageSprite?.name}, Duration: {_currentStage.GrowTimer}");

                // Устанавливаем спрайт для текущей стадии
                if (_currentStage.StageSprite != null)
                {
                    _renderer.sprite = _currentStage.StageSprite;
                }

                // Устанавливаем длительность для текущей стадии
                _growDuration = _currentStage.GrowTimer;
                _growProgress = 0f;

                // Переходим к следующей стадии
                _stagesCount++;

                Debug.Log($"[PlantBehavior] Next stage will be: {_stagesCount}, New grow duration: {_growDuration}");

                // Обновляем визуализацию прогресса
                _visualization?.InitializeProgress(this);
            }
            else
            {
                Debug.LogError($"[PlantBehavior] Current stage is null at index {_stagesCount}!");
            }
        }

        private void FixedUpdate()
        {
            if (_plantData != null && !_canHarvest)
            {
                TimerCheck();
            }
        }

        private void TimerCheck()
        {
            if (_canHarvest) return;
            if (_growthSpeed <= 0f) return;
            if (_growDuration <= 0f) return; // Защита от деления на ноль

            _growProgress += Time.fixedDeltaTime * _growthSpeed;

            if (_growProgress >= _growDuration)
            {
                Debug.Log($"[PlantBehavior] TimerCheck - Growth complete for stage. Progress: {_growProgress}, Duration: {_growDuration}");
                HandleGrow();
                _growProgress = 0f; // Сбрасываем прогресс здесь, а не в HandleGrow
            }
        }

        private IEnumerator WishEnableTick()
        {
            if (_growDuration <= 0)
            {
                Debug.LogWarning("[PlantBehavior] WishEnableTick - growDuration is 0, using default wait time");
                yield return new WaitForSeconds(5f);
            }
            else
            {
                float waitTime = UnityEngine.Random.Range(_growDuration / 4, _growDuration / 2);
                Debug.Log($"[PlantBehavior] WishEnableTick - Waiting {waitTime} seconds before creating wish");
                yield return new WaitForSeconds(waitTime);
            }

            _currentWish = WishFactory.Instance.CreateRandomWish();
            Debug.Log($"[PlantBehavior] Created wish: {_currentWish?.GetType().Name}");
            _visualization?.VisualizeWish(_currentWish);
        }

        // Для перезапуска (например, после сбора урожая и пересадки)
        public void ResetGrowth()
        {
            Debug.Log($"[PlantBehavior] ResetGrowth on {gameObject.name}");
            _growProgress = 0f;
            _stagesCount = 0;
            _canHarvest = false;
            _growthSpeed = 1f;

            if (_plantData != null && _plantData.NotReadyForHarvestSpr != null)
            {
                _renderer.sprite = _plantData.NotReadyForHarvestSpr;
            }

            // Переинициализируем первую стадию
            if (_plantData != null && _plantData.Stages.Count > 0)
            {
                HandleGrow();
            }
        }

        // Прогресс от 0 до 1 для UI
        public float GetGrowthProgress()
        {
            return _growDuration > 0f ? Mathf.Clamp01(_growProgress / _growDuration) : 1f;
        }

        // Для отладки в редакторе
        private void OnValidate()
        {
            if (_plantData != null && _plantData.Stages.Count > 0 && _growDuration == 0)
            {
                _growDuration = _plantData.Stages[0].GrowTimer;
                Debug.Log($"[PlantBehavior] OnValidate - Set initial growDuration to: {_growDuration}");
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (GameCondition.IsShovelModeEnabled)
            {
                HandleRemove();
                return;
            }

            if (_canHarvest)
                HandleHarvest();
        }

        private void HandleRemove()
        {
            if (_plantData == null) return; // пустой слот — нечего удалять

            if (_wishRoutine != null)
            {
                StopCoroutine(_wishRoutine);
                _wishRoutine = null;
            }

            _plantData = null;
            _canHarvest = false;
            _currentStage = null;
            _growProgress = 0f;
            _stagesCount = 0;
            _currentWish = null;

            if (_renderer != null)
                _renderer.sprite = null;

            _visualization?.OnHarvested(); // прячет прогресс-бар/иконку пожелания
        }
        private void HandleHarvest()
        {
            BakeryInventory.Add(_plantData.Ingredinet, _plantData.IngredientOutput);
            _visualization?.OnHarvested();
            ResetGrowth();
        }
    }
}
 