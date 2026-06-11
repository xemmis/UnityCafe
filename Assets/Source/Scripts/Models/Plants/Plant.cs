using Specs;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Models.Plant
{

    public sealed class Plant : MonoBehaviour
    {
        [SerializeField] private IPlantUIVisualizer _visualizer = null;

        private PlantSO _plantSO = null;
        public PlantSO PlantSO => _plantSO;

        private void Init(PlantSO plantSO)
        {
            _plantSO = plantSO;
        }

        private void Awake()
        {
            if (_visualizer == null)
            {
                _visualizer = GetComponentInChildren<IPlantUIVisualizer>();
            }
        }


    }


    public sealed class PlantUIVisualizer : MonoBehaviour, IPlantUIVisualizer
    {
        private Image _wishImage = null;

        private void Awake()
        {
            if (_wishImage == null)
            {
                _wishImage = GetComponentInChildren<Image>();
            }
        }

        public void VisualizeWish(PlantWishType plantWishType)
        {

        }

        public void VisualizeReadyCondition()
        {
            throw new NotImplementedException();
        }

        public void ClearUI()
        {
            throw new NotImplementedException();
        }
    }


    public enum PlantWishType
    {
        Food,
        Water,
        Attention
    }

}