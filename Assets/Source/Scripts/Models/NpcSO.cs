using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "Npc Data", menuName = "Npc/New Npc")]
    public class NpcSO : ScriptableObject
    {
        public Sprite NpcIcon { get; private set; } = null;
        public List<NpcAction> Actions { get; private set; } = new();

    }
}