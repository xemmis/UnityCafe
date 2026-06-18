using Specs;
using UnityEngine;
using UnityEngine.UI;

namespace Models.Plant
{
    public sealed class PlantUIVisualization : MonoBehaviour
    {
        [SerializeField] private Image _progressImage = null;
        [SerializeField] private Image _wishImage = null;

        private PlantBehavior _plantBehavior;
        private bool _progressEnabled = false;
        private Color _transparent = new Color(0, 0, 0, 0);
        private Color _visible = new Color(1, 1, 1, 1);

        private void Start()
        {
            if (_progressEnabled && _plantBehavior != null)
            {
                _progressImage.sprite = _plantBehavior.PlantData.PlantSprite;
                _progressImage.color = _visible;
            }
            else
            {
                _progressImage.color = _transparent;
            }
        }

        private void Update()
        {
            if (_progressEnabled)
                HandleVisualizeProgress();
        }

        public void VisualizeWish(IWish plantWish, bool condition = true)
        {
            if (condition)
            {
                _wishImage.color = _visible;
                _wishImage.sprite = plantWish.Icon;
            }
            else
            {
                _wishImage.color = _transparent;
            }
        }

        public void InitializeProgress(PlantBehavior plantBehavior)
        {
            _plantBehavior = plantBehavior;
            _progressEnabled = true;
            HandleVisualizeProgress();
        }

        private void HandleVisualizeProgress()
        {
            if (!_progressEnabled) return;

            _progressImage.fillAmount = _plantBehavior.GetGrowthProgress();

            if (_progressImage.fillAmount >= 1f)
            {
                _progressEnabled = false;
            }
        }
    }
}