using Specs;
using UnityEngine;
using UnityEngine.UI;

namespace Models.Plant
{
    public sealed class PlantUIVisualizer : MonoBehaviour, IPlantUIVisualizer
    {
        [SerializeField] private Image _wishImage = null;
        [SerializeField] private Sprite _foodIcon;
        [SerializeField] private Sprite _waterIcon;
        [SerializeField] private Sprite _attentionIcon;
        [SerializeField] private Sprite _readyIcon;

        [SerializeField] private GameObject _wishPanel;
        [SerializeField] private Color _readyColor = Color.green;
        [SerializeField] private Color _defaultColor = Color.white;

        private void Awake()
        {
            if (_wishImage == null)
            {
                _wishImage = GetComponentInChildren<Image>();
            }

            if (_wishPanel == null)
            {
                _wishPanel = _wishImage?.gameObject;
            }
        }

        public void VisualizeWish(PlantWishType plantWishType)
        {
            if (_wishPanel != null)
            {
                _wishPanel.SetActive(true);
            }

            if (_wishImage != null)
            {
                _wishImage.color = _defaultColor;

                switch (plantWishType)
                {
                    case PlantWishType.Food:
                        if (_foodIcon != null) _wishImage.sprite = _foodIcon;
                        break;
                    case PlantWishType.Water:
                        if (_waterIcon != null) _wishImage.sprite = _waterIcon;
                        break;
                    case PlantWishType.Attention:
                        if (_attentionIcon != null) _wishImage.sprite = _attentionIcon;
                        break;
                    case PlantWishType.None:
                    default:
                        ClearUI();
                        break;
                }
            }

            Debug.Log($"Visualizing wish: {plantWishType}");
        }

        public void VisualizeReadyCondition()
        {
            if (_wishPanel != null)
            {
                _wishPanel.SetActive(true);
            }

            if (_wishImage != null)
            {
                if (_readyIcon != null)
                {
                    _wishImage.sprite = _readyIcon;
                }
                _wishImage.color = _readyColor;
            }

            Debug.Log("Plant is ready to harvest!");
        }

        public void ClearUI()
        {
            if (_wishPanel != null)
            {
                _wishPanel.SetActive(false);
            }

            if (_wishImage != null)
            {
                _wishImage.sprite = null;
                _wishImage.color = _defaultColor;
            }

            Debug.Log("UI cleared");
        }
    }


}