using Core.Dialogue;
using Models.Food;
using System.Collections.Generic;
using UnityEngine;

namespace Models.Npc
{
    [System.Serializable]
    public sealed class NpcAction
    {
        [field: SerializeField] public DialogueTree Dialogue { get; private set; }
        [field: SerializeField] public StateType StateType { get; private set; }
        [field: SerializeField] public WalkType WalkType { get; private set; }
        [field: SerializeField] public FoodRecipe FoodRecipe { get; private set; }
        [field: SerializeField] public List<FoodPrefer> FoodPrefers { get; private set; } = new();
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