using Models.Npc;
using Models.States;
using Specs;
using UnityEngine;

namespace Core
{
    public static class NpcStateFabric
    {
        public static IState CreateState(NpcAction action)
        {
            switch (action.StateType)
            {
                case StateType.Idle:
                    return new IdleState(action.IntData);
                case StateType.Walk:
                    return new WalkState(action.WalkType);
                case StateType.MakeOrder:
                    return new MakeOrder(action.IntData);
                case StateType.Vibing:
                    Debug.Log("Vibing not realizet yet");
                    break;
                case StateType.Leave:
                    return new ExitState();
                case StateType.Cook:
                    return new MakeFood(action.FoodRecipe);
                case StateType.Quest:
                    return new QuestState();
                case StateType.WaitOrder:
                    return new WaitOrderState();
                default:
                    Debug.Log("State Is not Assigned To Fabric");
                    return null;
            }

            return null;
        }
    }
}
