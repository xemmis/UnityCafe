using System.Collections.Generic;
using UnityEngine;
namespace Core.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue Tree", menuName = "Dialogue Core/Tree")]
    public sealed class DialogueTree : ScriptableObject
    {
        public List<DialogueNode> Nodes = new();
        public DialogueNode GetNode(int index)
        {
            if (index < 0 || index >= Nodes.Count)
                return null;

            return Nodes[index];
        }
    }
}
