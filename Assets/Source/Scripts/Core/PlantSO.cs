namespace Core
{
    using Models.Food;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Plant", menuName = "Plant/Create New Plant")]
    public sealed class PlantSO : ScriptableObject
    {
        [field: SerializeField] public IngredientItem Ingredinet { get; private set; } = null;
        [field: SerializeField] public List<PlantStage> Stages { get; private set; } = new();
        [field: SerializeField] public int IngredientOutput { get; private set; } = 1;
        [field: SerializeField] public int PlantCost { get; private set; } = 15;
        [field: SerializeField] public Sprite ReadyForHarvestSpr { get; private set; } = null;
        [field: SerializeField] public Sprite NotReadyForHarvestSpr { get; private set; } = null;
        [field: SerializeField] public bool IsQuestReward { get; private set; } = false;
    }
}