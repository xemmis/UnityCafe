namespace Models.Furniture
{
    using Core;
    using UnityEngine;

    public sealed class FurnitureVisualizer
    {
        private readonly SpriteRenderer _furnitureRenderer;
        private readonly SpriteRenderer[] _dependentRenderers;
        private readonly FurnutureData _data;
        private bool _isSolded;
        private int _upgradeIndex;

        private static readonly Color BuildingMode = new(1, 1, 1, 0.4f);
        private static readonly Color Transparent = new(1, 1, 1, 0);
        private static readonly Color Visible = new(1, 1, 1, 1);

        public FurnitureVisualizer(SpriteRenderer spriteRenderer, SpriteRenderer[] dependentRenderers, FurnutureData furnutureData)
        {
            _data = furnutureData;
            _furnitureRenderer = spriteRenderer;
            _dependentRenderers = dependentRenderers ?? System.Array.Empty<SpriteRenderer>();

            GameCondition.OnBuilding += ChangeBuildingCondition;
        }

        public void ChangeBuildingCondition(bool condition)
        {
            _isSolded = _upgradeIndex > 0;

            if (condition) HandleVisualize();
            else HandleClear();
        }

        private void HandleVisualize()
        {
            if (_isSolded)
            {
                int currentIndex = _upgradeIndex - 1;
                bool hasNextSprite = _upgradeIndex < _data.FurnitureStages.Count
                                     && _data.FurnitureStages[_upgradeIndex].NextStageSprite != null;

                if (hasNextSprite)
                {
                    _furnitureRenderer.sprite = _data.FurnitureStages[_upgradeIndex].NextStageSprite;
                    _furnitureRenderer.color = BuildingMode;
                    UpdateDependents(_upgradeIndex, BuildingMode);
                }
                else
                {
                    _furnitureRenderer.sprite = _data.FurnitureStages[currentIndex].CurrentStageSprite;
                    _furnitureRenderer.color = Visible;
                    UpdateDependents(currentIndex, Visible);
                }
            }
            else
            {
                _furnitureRenderer.sprite = _data.FurnitureStages[0].CurrentStageSprite;
                _furnitureRenderer.color = BuildingMode;
                UpdateDependents(0, BuildingMode);
            }
        }

        private void HandleClear()
        {
            if (_isSolded)
            {
                int currentIndex = _upgradeIndex - 1;
                _furnitureRenderer.sprite = _data.FurnitureStages[currentIndex].CurrentStageSprite;
                _furnitureRenderer.color = Visible;
                UpdateDependents(currentIndex, Visible);
            }
            else
            {
                _furnitureRenderer.color = Transparent;
                HideDependents();
            }
        }

        private void UpdateDependents(int stageIndex, Color color)
        {
            if (_dependentRenderers.Length == 0) return;
            if (stageIndex >= _data.FurnitureStages.Count) return;

            Sprite[] sprites = _data.FurnitureStages[stageIndex].DependentSprites;

            for (int i = 0; i < _dependentRenderers.Length && i < sprites.Length; i++)
            {
                if (_dependentRenderers[i] != null && sprites[i] != null)
                {
                    _dependentRenderers[i].sprite = sprites[i];
                    _dependentRenderers[i].color = color;
                }
            }
        }

        private void HideDependents()
        {
            foreach (SpriteRenderer renderer in _dependentRenderers)
            {
                if (renderer != null)
                    renderer.color = Transparent;
            }
        }

        public void AddUpgradeIndex()
        {
            _upgradeIndex++;
            _isSolded = true;
        }

        public void ClearDependencis()
        {
            GameCondition.OnBuilding -= ChangeBuildingCondition;
        }
    }
}