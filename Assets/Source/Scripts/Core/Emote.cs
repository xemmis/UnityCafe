namespace Core
{
    using NUnit.Framework;
    using System;
    using UnityEngine;

    [Serializable]
    public sealed class Emote
    {
        [field: SerializeField] public EmoteType Type { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
    }
}
