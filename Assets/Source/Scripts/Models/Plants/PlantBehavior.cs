using Core;
using Specs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models.Plant
{
    public sealed class PlantBehavior : MonoBehaviour
    {
        [SerializeField] private float _growDuration = 150f;
        [SerializeField] private bool _canHarvest = false;
        [SerializeField] private PlantUIVisualization _visualization = null;

        private SpriteRenderer _renderer = null;
        private PlantSO _plantData = null;
        private Coroutine _wishRoutine = null;
        private IWish _currentWish = null;
        private float _growthSpeed = 1f;
        private float _growProgress;

        public bool CanHarvest => _canHarvest;

        public void SetGrowthSpeed(float speed) => _growthSpeed = Mathf.Max(0f, speed);
        public void PauseGrowth() => _growthSpeed = 0f;
        public void ResumeGrowth() => _growthSpeed = 1f;
        public void BoostGrowth(float multiplier) => _growthSpeed = multiplier;

        public void Initialize(PlantSO plant)
        {
            _plantData = plant;

            if (_renderer == null)
            {
                _renderer = GetComponent<SpriteRenderer>();
            }

            _renderer.sprite = _plantData.PlantSprite;
            _growDuration = _plantData.GrowTimer;

            _visualization?.InitializeProgress(this);
            _wishRoutine = StartCoroutine(WishEnableTick());
        }

        public void AddGrowthProgress(float seconds)
        {
            _growProgress = Mathf.Min(_growProgress + seconds, _growDuration);
            if (_growProgress >= _growDuration)
            {
                _canHarvest = true;
            }
        }

        private void FixedUpdate()
        {
            if (_plantData != null)
                TimerCheck();
        }

        private void TimerCheck()
        {
            if (_canHarvest) return;
            if (_growthSpeed <= 0f) return; // Остановка — не накапливаем прогресс

            _growProgress += Time.fixedDeltaTime * _growthSpeed;

            if (_growProgress >= _growDuration)
            {
                _growProgress = _growDuration;
                _canHarvest = true;
            }
        }

        private IEnumerator WishEnableTick()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(_growDuration / 4, _growDuration / 2));
            _currentWish = WishFactory.Instance.CreateRandomWish();

            _visualization.VisualizeWish(_currentWish);
        }

        public void HandleWishExecute()
        {
            if (_currentWish == null) return;

            _currentWish.Execute(this);
        }

        // Для перезапуска (например, после сбора урожая и пересадки)
        public void ResetGrowth()
        {
            _growProgress = 0f;
            _canHarvest = false;
        }

        // Прогресс от 0 до 1 для UI
        public float GetGrowthProgress() => _growDuration > 0f ? _growProgress / _growDuration : 1f;
    }
}