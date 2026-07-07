using System;
using UnityEngine;
namespace Core.Dialogue
{
    [Serializable]
    public struct QuestDialoguePair
    {
        [field: SerializeField] public int ProgressStage { get; private set; }
        [field: SerializeField] public DialogueTree Dialogue { get; private set; }
    }
}
