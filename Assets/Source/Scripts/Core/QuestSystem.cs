using Core;
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
            ValidateUniqueRewards();
        }

        private void InitializeDict()
        {
            foreach (QuestContainer quest in _quests)
            {
                if (quest == null) continue;
                _questDict[quest] = false;
            }
        }

        private void ValidateUniqueRewards()
        {
            var seen = new HashSet<PlantSO>();

            foreach (QuestContainer quest in _quests)
            {
                if (quest == null) continue;

                foreach (QuestDialoguePair pair in quest.Data.QuestDialogues)
                {
                    if (pair.RewardPlant == null) continue;

                    if (!seen.Add(pair.RewardPlant))
                        Debug.LogWarning($"[QuestSystem] Растение '{pair.RewardPlant.name}' назначено наградой больше одного раза — квестовые семена должны быть уникальными.");
                }
            }
        }

        public QuestContainer GetFreeQuest()
        {
            foreach (var pair in _questDict)
            {
                if (pair.Value) continue;
                if (!pair.Key.Data.HasNextStage()) continue;

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

        public void CompleteQuestStage(QuestContainer quest)
        {
            if (quest == null) return;
            quest.Data.AdvanceProgress();
        }
    }
}
  