using System.Collections.Generic;
using UnityEngine;

namespace Models.Npc
{
    [CreateAssetMenu(fileName = "Action Data", menuName = "Npc/New ActionList")]
    public sealed class ActionsSO : ScriptableObject
    {
        [field: SerializeField] public List<NpcActions> Actions { get; private set; } = new();

    }
}