namespace Core
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Npc", menuName = "Npc/New Npc")]
    public sealed class NpcData : ScriptableObject
    {
        [field: SerializeField] public GameObject Prefab { get; private set; } = null;
        [field: SerializeField] public EmoteContainer Emotes { get; private set; } = null;
    }
}
