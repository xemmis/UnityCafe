using System;
using UnityEngine;

namespace Models.Plant
{
    // Конфигурация для настройки желаний в инспекторе
    [Serializable]
    public sealed class WishConfig
    {
        public WishType Type;
        public Sprite Icon;
        public float TimeReduce;
    }
}