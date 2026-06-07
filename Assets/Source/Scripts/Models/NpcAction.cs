using Core.Dialogue;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [System.Serializable]
    public sealed class NpcAction
    {
        [field: SerializeField] public DialogueTree Dialogue { get; private set; }
        [field: SerializeField] public StateType StateType { get; private set; }
        [field: SerializeField] public WalkType WalkType { get; private set; }
        [field: SerializeField] public List<FoodPrefer> FoodTypes { get; private set; } = new();
        [field: SerializeField] public int IntData { get; private set; }
    }

    public enum FoodPrefer
    {
        Spicy,      // Острое 🌶️
        Sweet,      // Сладкое 🍰
        Crunchy,    // Хрустящее 🍪
        Salty,      // Солёное 🧂
        Sour,       // Кислое 🍋
        Bitter      // Горькое ☕
    }

    public enum WalkType
    {
        MakeOrder,
        WalkAround,
        Leave,
        Table,
        Work
    }
}