using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    [CreateAssetMenu(fileName = "Dialogue Tree", menuName = "Dialogue Core/Tree")]
    public sealed class DialogueTree : ScriptableObject
    {
        public List<DialogueNode> Nodes = new();
        public DialogueNode GetNode(int index)
        {
            if (Nodes[index] == null)
            {
                return null;
            }

            return Nodes[index];
        }
    }
}
