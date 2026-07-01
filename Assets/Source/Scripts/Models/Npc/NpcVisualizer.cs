

namespace Models.Npc
{
    using Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Utils;

    public sealed class NpcVisualizer
    {
        private readonly Image _wishImage;
        private Dictionary<EmoteType, Sprite> _emoteCache;

        public EmoteContainer EmoteContainer { get; private set; }
        private Coroutine _clearRoutine;
        private MonoBehaviour _owner;

        public NpcVisualizer(Image wishImage, MonoBehaviour owner)
        {
            _wishImage = wishImage;
            _owner = owner;
        }

        public void ClearAfterDelay(float delay)
        {
            if (_clearRoutine != null)
                _owner.StopCoroutine(_clearRoutine);

            _clearRoutine = _owner.StartCoroutine(ClearRoutine(delay));
        }

        private IEnumerator ClearRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            ClearEmote();
            _clearRoutine = null;
        }

        public void Initialize(EmoteContainer emoteContainer)
        {
            EmoteContainer = emoteContainer;

            if (emoteContainer == null || _emoteCache != null) return;

            _emoteCache = new Dictionary<EmoteType, Sprite>(emoteContainer.Emotes.Length);
            foreach (Emote emote in emoteContainer.Emotes)
                _emoteCache[emote.Type] = emote.Sprite;
        }

        public void SetSprite(Sprite sprite)
        {
            _wishImage.color = ColorExtensions.Visible();
            _wishImage.sprite = sprite;
        }

        public void ClearSprite()
        {
            _wishImage.sprite = null;
            _wishImage.color = ColorExtensions.Transparent;
        }

        public void SetEmote(EmoteType type)
        {
            if (_emoteCache != null && _emoteCache.TryGetValue(type, out Sprite sprite) && _wishImage != null)
            {
                _wishImage.sprite = sprite;
                _wishImage.color = ColorExtensions.Visible();
            }
        }

        public void ClearEmote()
        {
            if (_wishImage == null) return;
            _wishImage.sprite = null;
            _wishImage.color = ColorExtensions.Transparent;
        }
    }
}