namespace Core
{
    using Core.Dialogue;
    using Models.Npc;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class OrderFactory : MonoBehaviour
    {
        [SerializeField] private List<OrderPreferenceConfig> _configs = new();
        [SerializeField] private Vector2Int _prefersCountRange = new(1, 1); // управляешь тут в инспекторе

        public static OrderFactory Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // Удобная перегрузка — количество берётся из диапазона в инспекторе
        public (List<FoodPrefer> prefers, DialogueTree hint) CreateRandomOrder()
        {
            int count = Random.Range(_prefersCountRange.x, _prefersCountRange.y + 1);
            return CreateRandomOrder(count);
        }

        // Явный вызов с конкретным количеством (пригодится, если захочешь задавать per-NPC)
        public (List<FoodPrefer> prefers, DialogueTree hint) CreateRandomOrder(int prefersCount)
        {
            List<FoodPrefer> prefers = new();
            DialogueTree hint = null;

            if (_configs.Count == 0)
            {
                Debug.LogError("[OrderFactory] Нет ни одной конфигурации предпочтений!");
                prefers.Add(FoodPrefer.Sweet);
                return (prefers, hint);
            }

            // Не больше, чем есть уникальных конфигов — иначе зациклимся/задублируем
            prefersCount = Mathf.Clamp(prefersCount, 1, _configs.Count);

            List<OrderPreferenceConfig> pool = new(_configs);

            for (int i = 0; i < prefersCount; i++)
            {
                int index = Random.Range(0, pool.Count);
                OrderPreferenceConfig config = pool[index];
                pool.RemoveAt(index); // без повторов — NPC не может хотеть "сладкое" дважды

                prefers.Add(config.Prefer);

                if (hint == null && config.HintTrees is { Length: > 0 })
                    hint = config.HintTrees[Random.Range(0, config.HintTrees.Length)];
            }

            return (prefers, hint);
        }
    }
}