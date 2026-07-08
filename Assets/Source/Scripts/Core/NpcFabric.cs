using Core.Dialogue;
using Models.Npc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Core
{
    public sealed class NpcFabric : MonoBehaviour
    {
        [SerializeField] private List<NpcData> _npcDatas = new();
        [SerializeField] private int _initialPoolSize = 5;
        [SerializeField] private int _maxPoolSize = 20;
        [SerializeField] private Vector2 _spawnTick = new(5f, 15f);
        [SerializeField] private int _maxActiveNpcs = 5;
        [SerializeField] private ActionsSO _actionsSo = null;

        [Header("Quests")]
        [SerializeField, Range(0f, 1f)] private float _questChance = 0.3f;

        private readonly Dictionary<NpcData, ObjectPool<NpcBehaviorLogic>> _pools = new();
        private readonly Dictionary<NpcBehaviorLogic, NpcData> _activeNpcs = new();
        private readonly Dictionary<NpcBehaviorLogic, QuestContainer> _activeQuests = new();
        private List<NpcData> _validDatas = new();
        private Coroutine _spawnRoutine;

        public static NpcFabric Instance { get; private set; } = null;

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

            foreach (NpcData npcData in _npcDatas)
            {
                if (npcData.Prefab == null)
                {
                    Debug.LogWarning($"[NpcFabric] NpcData '{npcData.name}' has no prefab — skipping.");
                    continue;
                }

                NpcBehaviorLogic logic = npcData.Prefab.GetComponent<NpcBehaviorLogic>();
                if (logic == null)
                {
                    Debug.LogError($"[NpcFabric] Prefab '{npcData.Prefab.name}' missing NpcBehaviorLogic — skipping.");
                    continue;
                }

                var pool = new ObjectPool<NpcBehaviorLogic>(logic, _initialPoolSize, _maxPoolSize, transform);
                _pools[npcData] = pool;
                _validDatas.Add(npcData);
            }
        }

        private void Start()
        {
            if (_validDatas.Count == 0)
            {
                Debug.LogWarning("[NpcFabric] No valid NpcData — spawn routine won't start.");
                return;
            }

            _spawnRoutine = StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_spawnTick.x, _spawnTick.y));

                if (_activeNpcs.Count >= _maxActiveNpcs) continue;

                NpcData randomData = _validDatas[Random.Range(0, _validDatas.Count)];

                // решаем, дать ли квест, ДО спавна, чтобы сразу передать в Initialize
                QuestContainer quest = RollQuest();

                NpcBehaviorLogic npc = Spawn(randomData, quest);

                if (npc != null)
                {
                    _activeNpcs.Add(npc, randomData);
                    npc.OnDespawn += HandleNpcDespawn;

                    if (quest != null)
                        _activeQuests.Add(npc, quest);
                }
                else
                {
                    // спавн не удался — освобождаем взятый квест, иначе он "зависнет" занятым навсегда
                    if (quest != null)
                        QuestSystem.Instance?.ReleaseQuest(quest);
                }
            }
        }

        private QuestContainer RollQuest()
        {
            if (QuestSystem.Instance == null) return null;
            if (Random.value > _questChance) return null;

            return QuestSystem.Instance.GetFreeQuest();
        }

        private void HandleNpcDespawn(NpcBehaviorLogic npc)
        {
            npc.OnDespawn -= HandleNpcDespawn;

            if (_activeQuests.TryGetValue(npc, out QuestContainer quest))
            {
                _activeQuests.Remove(npc);
                QuestSystem.Instance?.ReleaseQuest(quest);
            }

            if (_activeNpcs.TryGetValue(npc, out NpcData data))
            {
                _activeNpcs.Remove(npc);
                Despawn(data, npc);
            }
        }

        public NpcBehaviorLogic Spawn(NpcData npcData, QuestContainer questContainer = null)
        {
            if (!_pools.TryGetValue(npcData, out ObjectPool<NpcBehaviorLogic> pool))
            {
                Debug.LogError($"[NpcFabric] No pool for NpcData '{npcData.name}'.");
                return null;
            }

            NpcBehaviorLogic npc = pool.Get();
            npc.Initialize(_actionsSo, npcData.Emotes, questContainer);
            return npc;
        }

        public void Despawn(NpcData npcData, NpcBehaviorLogic npc)
        {
            if (!_pools.TryGetValue(npcData, out ObjectPool<NpcBehaviorLogic> pool))
            {
                Debug.LogError($"[NpcFabric] No pool for NpcData '{npcData.name}'.");
                return;
            }

            pool.Return(npc);
        }

        private void OnDestroy()
        {
            if (_spawnRoutine != null)
                StopCoroutine(_spawnRoutine);

            foreach (NpcBehaviorLogic npc in new List<NpcBehaviorLogic>(_activeNpcs.Keys))
                npc.OnDespawn -= HandleNpcDespawn;

            foreach (var pool in _pools.Values)
                pool.Dispose();

            _pools.Clear();
            _activeNpcs.Clear();
            _activeQuests.Clear();
            _validDatas.Clear();
        }
    }
}