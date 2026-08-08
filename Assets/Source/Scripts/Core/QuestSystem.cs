using System.Collections.Generic;
using UnityEngine;

namespace Core.Dialogue
{
    public sealed class QuestSystem : MonoBehaviour
    {
        [SerializeField] private List<QuestContainer> _quests = new();

        // ключ - квест, значение - занят он сейчас или нет
        private readonly Dictionary<QuestContainer, bool> _questDict = new();

        public static QuestSystem Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeDict();
        }

        private void InitializeDict()
        {
            foreach (QuestContainer quest in _quests)
            {
                if (quest == null) continue;
                _questDict[quest] = false; // false = свободен
            }
        }

        /// <summary>
        /// Возвращает свободный квест и сразу помечает его занятым.
        /// Если свободных нет - вернёт null.
        /// </summary>
        public QuestContainer GetFreeQuest()
        {
            foreach (var pair in _questDict)
            {
                if (pair.Value == false)
                {
                    _questDict[pair.Key] = true;
                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Освободить квест (например, если нпс despawn-нулся, не завершив его).
        /// </summary>
        public void ReleaseQuest(QuestContainer quest)
        {
            if (quest == null) return;

            if (_questDict.ContainsKey(quest))
                _questDict[quest] = false;
        }

        /// <summary>
        /// Пометить квест как завершённый/навсегда занятым - если нужна такая логика.
        /// </summary>
        public void CompleteQuest(QuestContainer quest)
        {
            // тут можешь добавить свою логику прогресса,
            // например quest.Data.AdvanceProgress() и т.п.
        }
    }
}