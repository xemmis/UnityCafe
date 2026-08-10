using System;
using Core;
using Models.Food;
using UnityEngine;

namespace Core.Dialogue
{
    [Serializable]
    public struct QuestDialoguePair
    {
        [field: SerializeField] public int ProgressStage { get; private set; }
        [field: SerializeField] public DialogueTree Dialogue { get; private set; }

        // Предмет, который NPC должен получить на этой стадии квеста
        [field: SerializeField] public FoodItem RequiredItem { get; private set; }

        // Семечко, которое игрок получает за прохождение этой стадии
        [field: SerializeField] public PlantSO RewardPlant { get; private set; }
    }
}