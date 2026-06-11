using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models.Npc
{
    [Serializable]
    public sealed class NpcActions
    {
        [field: SerializeField] public List<NpcAction> Actions { get; set; } = new();
    }
}