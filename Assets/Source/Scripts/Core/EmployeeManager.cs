using Models;
using System.Collections.Generic;

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
                if (npc.HasActions()) continue;

                npc.Initialize(Recipe.NpcActions);
            }
        }
    }
}