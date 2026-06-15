using Models.Plant;
using Specs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class WishFactory : MonoBehaviour
    {
        [SerializeField] private List<WishConfig> _wishConfigs;

        private Dictionary<WishType, Func<IWish>> _wishCreators;
        public static WishFactory Instance { get; private set; } = null;

        private void Awake()
        {
            InitializeSingleton();
            InitializeFactory();
        }

        private void InitializeSingleton()
        {
            if (WishFactory.Instance == null)
            {
                WishFactory.Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeFactory()
        {
            _wishCreators = new Dictionary<WishType, Func<IWish>>();

            foreach (var config in _wishConfigs)
            {
                if (config.Type == WishType.None) continue;

                switch (config.Type)
                {
                    case WishType.Food:
                        _wishCreators[WishType.Food] = () => new FoodWish(config.Icon, config.TimeReduce);
                        break;
                    case WishType.Water:
                        _wishCreators[WishType.Water] = () => new WaterWish(config.Icon, config.TimeReduce);
                        break;
                    case WishType.Attention:
                        _wishCreators[WishType.Attention] = () => new AttentionWish(config.Icon, config.TimeReduce);
                        break;
                }
            }
        }

        // Основной метод получения желания по типу
        public IWish CreateWish(WishType type)
        {
            if (_wishCreators.TryGetValue(type, out var creator))
            {
                return creator();
            }

            Debug.LogError($"WishFactory: Нет конфигурации для типа {type}");
            return null;
        }

        // Получить случайное желание (кроме None)
        public IWish CreateRandomWish()
        {
            var availableTypes = new List<WishType>(_wishCreators.Keys);
            if (availableTypes.Count == 0) return null;

            var randomType = availableTypes[UnityEngine.Random.Range(0, availableTypes.Count)];
            return CreateWish(randomType);
        }

        // Проверить, есть ли конфигурация для типа
        public bool HasWishType(WishType type) => _wishCreators.ContainsKey(type);
    }
}
