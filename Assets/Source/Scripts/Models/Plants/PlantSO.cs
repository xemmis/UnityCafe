using UnityEngine;

namespace Models.Plant
{
    [CreateAssetMenu(fileName = "new PlantSO", menuName = "Plant/New Plant")]
    public class PlantSO : ScriptableObject
    {
        [field: SerializeField] public Sprite ReadyIcon { get; private set; } = null;
        [field: SerializeField] public GameObject PlantPrefab { get; private set; } = null;
        [field: SerializeField] public int GrowTime { get; private set; }
        [field: SerializeField] public Vector2 _earnRange { get; private set; }
    }

}