using Models;
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
                    Debug.Log("MakeOrder not realizet yet");
                    break;
                case StateType.Vibing:
                    Debug.Log("Vibing not realizet yet");
                    break;
                case StateType.Leave:
                    Debug.Log("Leave not realizet yet");
                    break;
                default:
                    Debug.Log("State Is not Assigned To Fabric");
                    return null;
            }

            return null;
        }
    }
}
