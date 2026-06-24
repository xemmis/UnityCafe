using Models.Furniture;
using Specs;
using UnityEngine.UI;

namespace Models.Furniture
{
    using Core;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Furniture : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private FurnutureData _data = null;
        [SerializeField] private List<BuyableWalkPoint> _points = new();
        [SerializeField] private int _cost = 15;

        private FurnitureVisualizer _visualizer;
        private FurnitureShaker _shaker;
        private SpriteRenderer _renderer;
        private int _currentStageIndex;
        private bool _registerFlag = false;
        private bool IsSold => _currentStageIndex > 0;
        private bool HasNextStage => _currentStageIndex < _data.FurnitureStages.Count
                                     && _data.FurnitureStages[_currentStageIndex].NextStageSprite != null;

        [SerializeField] private SpriteRenderer[] _dependentRenderers;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _visualizer = new FurnitureVisualizer(_renderer, _dependentRenderers, _data);
            _shaker = new FurnitureShaker(transform, transform.localScale);
        }

        private void Start()
        {
            _visualizer.ChangeBuildingCondition(GameCondition.IsBuildingModeEnabled);
        }

        private void OnDestroy()
        {
            _visualizer.ClearDependencis();
            _shaker.ClearDependencis();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!GameCondition.IsBuildingModeEnabled) return;
            if (IsSold && !HasNextStage) return;

            ConfirmBuy();
        }

        private void ConfirmBuy()
        {
            if (!Wallet.TrySpendMoney(_data.FurnitureStages[_currentStageIndex].NextStageCost))
                return;

            _currentStageIndex++;
            _visualizer.AddUpgradeIndex();
            if (!_registerFlag)
            {
                foreach (WalkPoint walkPoint in _points)
                    WalkManager.Instance.RegisterPoint(walkPoint);

                _registerFlag = true;
            }

            _shaker.StopShaking();
            GameCondition.ChangeBuildingModeCondition(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!GameCondition.IsBuildingModeEnabled) return;
            if (IsSold && !HasNextStage) return;

            _shaker.StartShake();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _shaker.StopShaking();
        }
    }
}
