// Core/OrderFactory.cs
namespace Core
{
    using Core.Dialogue;
    using Models.Npc;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class OrderFactory : MonoBehaviour
    {
        [SerializeField] private List<OrderPreferenceConfig> _configs = new();
        public static OrderFactory Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public (FoodPrefer prefer, DialogueTree hint) CreateRandomOrder()
        {
            if (_configs.Count == 0)
            {
                Debug.LogError("[OrderFactory] Нет ни одной конфигурации предпочтений!");
                return (FoodPrefer.Sweet, null);
            }

            OrderPreferenceConfig config = _configs[Random.Range(0, _configs.Count)];
            DialogueTree hint = config.HintTrees is { Length: > 0 }
                ? config.HintTrees[Random.Range(0, config.HintTrees.Length)]
                : null;

            return (config.Prefer, hint);
        }
    }
}