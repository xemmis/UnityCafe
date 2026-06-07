using System;
namespace Core.Dialogue
{
    [Serializable]
    public sealed class MoodDialogueEntry
    {
        public DialogueMood Mood;
        public DialogueTree[] Trees;
    }
}
