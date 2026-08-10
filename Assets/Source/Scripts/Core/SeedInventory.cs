using System;
using System.Collections.Generic;
using UnityEngine;


namespace Core
{
    public static class SeedInventory
    {
        private static readonly Dictionary<PlantSO, int> _seeds = new();

        public static IReadOnlyDictionary<PlantSO, int> Seeds => _seeds;

        public static event Action<PlantSO, int> OnSeedInventoryChange;

        public static int GetAmount(PlantSO seed) =>
            seed != null && _seeds.TryGetValue(seed, out int amount) ? amount : 0;

        public static void Add(PlantSO seed, int amount = 1)
        {
            if (!ValidateInput(seed, amount, "добавить")) return;

            _seeds[seed] = GetAmount(seed) + amount;
            OnSeedInventoryChange?.Invoke(seed, _seeds[seed]);
        }

        public static bool Remove(PlantSO seed, int amount = 1)
        {
            if (!ValidateInput(seed, amount, "удалить")) return false;

            int current = GetAmount(seed);
            if (current < amount)
            {
                Debug.LogWarning($"[SeedInventory] Не хватает семян {seed.name}: нужно {amount}, есть {current}");
                return false;
            }

            _seeds[seed] = current - amount;
            if (_seeds[seed] == 0) _seeds.Remove(seed);

            OnSeedInventoryChange?.Invoke(seed, GetAmount(seed));
            return true;
        }

        public static Dictionary<PlantSO, int> GetAllSeeds() => new(_seeds);

        private static bool ValidateInput(PlantSO seed, int amount, string action)
        {
            if (seed == null)
            {
                Debug.LogWarning($"[SeedInventory] Попытка {action} null-объект");
                return false;
            }
            if (amount <= 0)
            {
                Debug.LogWarning($"[SeedInventory] Количество должно быть > 0, получено: {amount}");
                return false;
            }
            return true;
        }
    }
}