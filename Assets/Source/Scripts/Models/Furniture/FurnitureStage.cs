namespace Models.Furniture
{
    using System;
    using UnityEngine;

    [Serializable]
    public sealed class FurnitureStage
    {
        [field: SerializeField] public Sprite CurrentStageSprite { get; private set; }
        [field: SerializeField] public Sprite NextStageSprite { get; private set; }
        [field: SerializeField] public int NextStageCost { get; private set; } = 0;

        [Header("Dependent Objects")]
        [field: SerializeField] public Sprite[] DependentSprites { get; private set; } = System.Array.Empty<Sprite>();
    }
}