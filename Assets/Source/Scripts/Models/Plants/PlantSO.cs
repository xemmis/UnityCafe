using Models.Plant;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plant", menuName = "Plants/Plant SO")]
public sealed class PlantSO : ScriptableObject
{
    [Header("Growth Settings")]
    public float BaseGrowthTime = 60f;
    public float MinGrowthTime = 10f;
    public float GrowthReductionPerWish = 0.1f; // 10% reduction per wish
    public int WishesToGrow = 5;

    [Header("Wish Settings")]
    public float MinWishInterval = 5f;
    public float MaxWishInterval = 15f;
    public WishWeight[] WishWeights;

    [Header("Visual Settings")]
    public Sprite PlantIcon;
    public string PlantName;
    public GameObject GrowthStages; // Можно добавить стадии роста
}

[Serializable]
public struct WishWeight
{
    public PlantWishType WishType;
    [Range(0f, 1f)]
    public float Weight;

    // Дополнительный конструктор для удобства
    public WishWeight(PlantWishType wishType, float weight)
    {
        WishType = wishType;
        Weight = Mathf.Clamp01(weight);
    }

    // Для удобной инициализации
    public static WishWeight Create(PlantWishType type, float weight)
        => new WishWeight(type, weight);
}