using Core;
using UnityEngine;

namespace Specs
{
    public interface IDialogueVisualizer
    {
        void Visualize(DialogueNode node, Sprite sprite = null);
        bool IsRevealing();
        void SkipReveal();
        void ClearText();
    }

    public interface ISelectable
    {
        void Select(bool condition);
    }
}
