namespace Core
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "EmoteContainer", menuName = "Npc/Emote Container")]
    public sealed class EmoteContainer : ScriptableObject
    {
        [field: SerializeField] public Emote[] Emotes { get; private set; }
    }
}
