namespace Models.Furniture
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName ="New Furniture",menuName ="Furniture/New Furniture")]
    public sealed class FurnutureData : ScriptableObject
    {
        [field: SerializeField] public List<FurnitureStage> FurnitureStages { get; set; } = new();
    }
}