namespace Core
{
    using UnityEngine;

    [System.Serializable]
    public class PlantStage
    {
        [field: SerializeField] public Sprite StageSprite { get; private set; } = null;
        [field: SerializeField] public float GrowTimer { get; private set; } = 15f;
    }

}