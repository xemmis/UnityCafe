using Specs;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Models.Plant
{
    public sealed class PlantUIVisualization : MonoBehaviour
    {
        [SerializeField] private Image _progressImage = null;
        [SerializeField] private Image _wishImage = null;
        [SerializeField] private GameObject _popupObject = null; // Сам объект попапа

        private PlantBehavior _plantBehavior;
        private bool _progressEnabled = false;
        private bool _hasWish = false;

        private void Start()
        {
            // Изначально скрываем все
            HideAll();
        }

        private void Update()
        {
            if (_progressEnabled && _plantBehavior != null)
            {
                HandleVisualizeProgress();
            }
        }

        private void HandleVisualizeProgress()
        {
            if (_plantBehavior == null) return;

            float progress = _plantBehavior.GetGrowthProgress();
            _progressImage.fillAmount = progress;

            // Если растение готово к сбору
            if (_plantBehavior.CanHarvest)
            {
                ShowHarvestReady();
            }
            // Если есть активное пожелание
            else if (_hasWish)
            {
                ShowWish();
            }
            // Иначе скрываем всё
            else
            {
                HidePopup();
            }
        }

        public void VisualizeWish(IWish plantWish, bool show = true)
        {
            if (show && plantWish != null)
            {
                _hasWish = true;
                _wishImage.sprite = plantWish.Icon;
                ShowWish();
                Debug.Log($"[PlantUIVisualization] Showing wish: {plantWish.GetType().Name}");
            }
            else
            {
                _hasWish = false;
                HideWish();
            }
        }

        public void InitializeProgress(PlantBehavior plantBehavior)
        {
            _plantBehavior = plantBehavior;
            _progressEnabled = true;

            // Устанавливаем иконку ингредиента для прогресс-бара
            if (_plantBehavior.PlantData != null && _plantBehavior.PlantData.Ingredinet != null)
            {
                _progressImage.sprite = _plantBehavior.PlantData.Ingredinet.Icon;
                Debug.Log($"[PlantUIVisualization] Progress icon set to: {_plantBehavior.PlantData.Ingredinet.Icon?.name}");
            }

            // Показываем прогресс-бар
            _progressImage.color = ColorExtensions.Visible();

            // Скрываем всё остальное
            HidePopup();

            Debug.Log($"[PlantUIVisualization] Progress initialized for {plantBehavior.gameObject.name}");
        }

        private void ShowHarvestReady()
        {
            if (_popupObject != null)
            {
                _popupObject.SetActive(true);
            }

            // Показываем иконку ингредиента в прогресс-баре
            _progressImage.color = ColorExtensions.Visible();

            // Скрываем иконку пожелания, если она была
            _wishImage.color = ColorExtensions.Transparent;

            // Можно добавить анимацию или эффект
            if (!_progressImage.gameObject.activeSelf)
            {
                _progressImage.gameObject.SetActive(true);
            }
        }

        private void ShowWish()
        {
            if (_popupObject != null)
            {
                _popupObject.SetActive(true);
            }

            // Показываем иконку пожелания
            _wishImage.color = ColorExtensions.Visible();

            // Прогресс-бар можно скрыть или оставить
            _progressImage.color = ColorExtensions.Transparent;
        }

        private void HideWish()
        {
            _wishImage.color = ColorExtensions.Transparent;
            _wishImage.sprite = null;

            // Если растение не готово к сбору, скрываем попап
            if (_plantBehavior == null || !_plantBehavior.CanHarvest)
            {
                HidePopup();
            }
        }

        private void HidePopup()
        {
            if (_popupObject != null)
            {
                _popupObject.SetActive(false);
            }
        }

        private void HideAll()
        {
            _progressImage.color = ColorExtensions.Transparent;
            _wishImage.color = ColorExtensions.Transparent;
            HidePopup();
        }

        // Вызывается когда пожелание выполнено
        public void OnWishFulfilled()
        {
            Debug.Log("[PlantUIVisualization] Wish fulfilled, hiding wish visualization");
            VisualizeWish(null, false);
            _hasWish = false;
        }

        // Вызывается когда растение собрано
        public void OnHarvested()
        {
            Debug.Log("[PlantUIVisualization] Plant harvested, resetting visualization");
            _progressEnabled = false;
            _hasWish = false;
            HideAll();
        }

        private void OnDisable()
        {
            HideAll();
        }

        private void OnDestroy()
        {
            HideAll();
        }
    }
}