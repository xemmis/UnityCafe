using Models;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public sealed class WalkManager : MonoBehaviour
    {
        public static WalkManager Instance = null;
        private Dictionary<WalkType, WalkPoint> _walkPointDict = new Dictionary<WalkType, WalkPoint>();

        private void Awake()
        {
            InitializeSingleton();
        }

        public void RegisterPoint(WalkPoint walkPoint, WalkType walkType)
        {
            _walkPointDict.Add(walkType, walkPoint);
        }

        private void InitializeSingleton()
        {
            if (WalkManager.Instance == null)
            {
                WalkManager.Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public WalkPoint GetWalkPoint(WalkType walkType)
        {
            _walkPointDict.TryGetValue(walkType, out WalkPoint result);
            return result;
        }
    }
}