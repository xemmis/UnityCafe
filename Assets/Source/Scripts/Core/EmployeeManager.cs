using Models.Food;
using Models.Npc;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public static class EmployeeManager
    {
        private static List<NpcBehaviorLogic> _employeeList = new();

        public static void RegisterEmployee(NpcBehaviorLogic npcBehaviorLogic)
        {
            _employeeList.Add(npcBehaviorLogic);
        }

        public static void SetWork(FoodRecipe Recipe)
        {
            foreach (NpcBehaviorLogic npc in _employeeList)
            {
                if (npc.IsWorking)
                {
                    Debug.Log("Has");
                    continue;
                }
                npc.SetWorkState(true, Recipe);
            }
        }
    }
}
