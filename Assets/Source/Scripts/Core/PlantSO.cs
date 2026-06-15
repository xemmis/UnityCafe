namespace Core
{
    using Models.Food;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Plant", menuName = "Plant/Create New Plant")]
    public sealed class PlantSO : ScriptableObject
    {
        [field: SerializeField] public IngredientItem Ingredinet { get; private set; } = null;
        [field: SerializeField] public Sprite PlantSprite { get; private set; } = null;
        [field: SerializeField] public float GrowTimer { get; private set; } = 150f;
        [field: SerializeField] public int IngredientOutput { get; private set; } = 1;
    }

}