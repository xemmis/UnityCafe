using System.Collections.Generic;
using UnityEngine;

namespace Core.Dialogue
{
    public sealed class QuestSystem : MonoBehaviour
    {
        [SerializeField] private List<QuestContainer> _quests = new();

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
                _questDict[quest] = false;
            }
        }

        /// <summary>
        /// Возвращает свободный квест, у которого ещё есть непройденная стадия.
        /// Полностью пройденные квесты больше не выдаются.
        /// </summary>
        public QuestContainer GetFreeQuest()
        {
            foreach (var pair in _questDict)
            {
                if (pair.Value) continue;                    // занят другим NPC
                if (!pair.Key.Data.HasNextStage()) continue;  // квест пройден целиком

                _questDict[pair.Key] = true;
                return pair.Key;
            }

            return null;
        }

        public void ReleaseQuest(QuestContainer quest)
        {
            if (quest == null) return;

            if (_questDict.ContainsKey(quest))
                _questDict[quest] = false;
        }

        /// <summary>
        /// Вызывается при успешной передаче нужного предмета NPC.
        /// Продвигает квест на следующую стадию.
        /// </summary>
        public void CompleteQuestStage(QuestContainer quest)
        {
            if (quest == null) return;
            quest.Data.AdvanceProgress();
        }
    }
}