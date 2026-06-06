using Core;

namespace Models
{
    [System.Serializable]
    public sealed class NpcAction
    {
        public DialogueTree Dialogue { get; private set; }
        public StateType StateType { get; private set; }
    }
}