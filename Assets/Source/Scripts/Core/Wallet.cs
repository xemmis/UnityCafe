using UnityEngine;

namespace Core
{
    public static class Wallet
    {
        public static int MoneyAmount { get; private set; }
        public static int DayEarnings { get; private set; }

        public static bool TrySpendMoney(int cost)
        {
            Debug.Log(cost);
            if (MoneyAmount - cost >= 0)
            {
                SpendMoney(cost);
                return true;
            }

            return false;
        }

        private static void SpendMoney(int cost)
        {
            MoneyAmount -= cost;
        }

        public static void AddMoney(int amount)
        {
            MoneyAmount += amount;
            DayEarnings += amount;
        }


        public static void ClearDayEarn()
        {
            DayEarnings = 0;
        }
    }
}