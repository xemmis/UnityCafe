using System;
namespace Core
{
    [Serializable]
    public class MoodDialogueEntry
    {
        public DialogueMood Mood;
        public DialogueTree[] Trees;
    }
}
