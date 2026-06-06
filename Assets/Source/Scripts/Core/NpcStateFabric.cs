using Models;
using NUnit.Framework;
using Specs;
using UnityEngine;

namespace Core
{
    public static class NpcStateFabric
    {
        public static IState CreateState(StateType type)
        {
            switch (type)
            {
                case StateType.Idle:
                    Debug.Log("Idle not realizet yet");
                    break;
                case StateType.Walk:
                    Debug.Log("Walk not realizet yet");
                    break;
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
