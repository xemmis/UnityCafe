using Core.Dialogue;
using System;
using UnityEngine;

namespace Models.Npc
{
    /// <summary>
    /// Числовое настроение NPC. Хранит "очки" настроения в диапазоне
    /// [-3; 3] и по ним определяет DialogueMood (Good/Neutral/Bad),
    /// который уже используется системой диалогов для выбора дерева.
    /// </summary>
    public sealed class NpcMood
    {
        private const int MinScore = -3;
        private const int MaxScore = 3;
        private const int BadThreshold = -2;
        private const int GoodThreshold = 2;

        private int _score;

        public DialogueMood Current { get; private set; }
        public event Action<DialogueMood> OnMoodChanged;

        public NpcMood(int startScore)
        {
            _score = Mathf.Clamp(startScore, MinScore, MaxScore);
            Current = ScoreToMood(_score);
        }

        public static NpcMood CreateRandom()
        {
            return new NpcMood(UnityEngine.Random.Range(MinScore, MaxScore + 1));
        }

        public void Improve(int amount = 1) => ChangeScore(amount);
        public void Worsen(int amount = 1) => ChangeScore(-amount);

        private void ChangeScore(int delta)
        {
            int newScore = Mathf.Clamp(_score + delta, MinScore, MaxScore);
            if (newScore == _score) return;

            _score = newScore;
            DialogueMood newMood = ScoreToMood(_score);

            if (newMood == Current) return;

            Current = newMood;
            OnMoodChanged?.Invoke(Current);
        }

        private static DialogueMood ScoreToMood(int score)
        {
            if (score <= BadThreshold) return DialogueMood.Bad;
            if (score >= GoodThreshold) return DialogueMood.Good;
            return DialogueMood.Neutral;
        }
    }
}