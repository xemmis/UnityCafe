using Models.Food;
using Models.Npc;
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
                if (npc.IsWorking)
                {
                    continue;
                }
                npc.SetWorkState(true);
                npc.InitializeActions(Recipe.NpcActions);
            }
        }
    }
}
 