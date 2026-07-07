using UnityEngine;
namespace Core.Dialogue
{
    [CreateAssetMenu(fileName = "Quest Container", menuName = "Dialogue Core/Quests")]
    public sealed class QuestContainer : ScriptableObject
    {
        [field: SerializeField] public QuestData Data { get; private set; } = null;

    }
}
