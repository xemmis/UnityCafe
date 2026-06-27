using Models.Npc;
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
        [SerializeField] private ActionsSO _actionsSo = null;
        private readonly Dictionary<NpcData, ObjectPool<NpcBehaviorLogic>> _pools = new();
        public static NpcFabric Instance { get; private set; } = null;

        private void Awake()
        {
            if (NpcFabric.Instance == null)
            {
                NpcFabric.Instance = this;
            }
            else
            {
                Destroy(gameObject);
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
            }
        }

        public NpcBehaviorLogic Spawn(NpcData npcData)
        {
            if (!_pools.TryGetValue(npcData, out ObjectPool<NpcBehaviorLogic> pool))
            {
                Debug.LogError($"[NpcFabric] No pool for NpcData '{npcData.name}'.");
                return null;
            }

            NpcBehaviorLogic npc = pool.Get();
            npc.Initialize(_actionsSo, npcData.Emotes);
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
            foreach (var pool in _pools.Values)
                pool.Dispose();

            _pools.Clear();
        }
    }
}