using Core.Dialogue;
using UnityEngine;

namespace Specs
{
    public interface IDialogueVisualizer
    {
        void Open(DialogueNode node, Sprite sprite = null);
        void ShowNode(DialogueNode node, Sprite sprite = null);
        void Close();
        bool IsRevealing();
        void SkipReveal();
    }
}