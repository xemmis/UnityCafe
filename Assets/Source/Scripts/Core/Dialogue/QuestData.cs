using System;
using System.Collections.Generic;
using UnityEngine;
namespace Core.Dialogue
{
    [Serializable]
    public class QuestData
    {
        [field: SerializeField] public int QuestProgress { get; private set; } = 0;

        [field: SerializeField] public List<QuestDialoguePair> QuestDialogues { get; private set; } = new();

        public DialogueTree GetDialogueForProgress(int progress)
        {
            foreach (var pair in QuestDialogues)
            {
                if (pair.ProgressStage == progress)
                    return pair.Dialogue;
            }
            return null;
        }

        public DialogueTree GetDialogueForProgress()
        {
            return GetDialogueForProgress(QuestProgress);
        }
    }
}
