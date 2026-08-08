namespace Models.Npc
{
    using Core;
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
        private Coroutine _popRoutine;
        private Coroutine _dismissRoutine;
        private MonoBehaviour _owner;

        // Pop-in
        private const float PopDuration = 0.35f;
        private const float OvershootScale = 1.25f;
        private const float FinalScale = 1.0f;

        // Dismiss
        private const float DismissDuration = 0.25f;
        private const float DismissShrinkScale = 0.6f; // до какого масштаба сжимается перед исчезновением

        public NpcVisualizer(Image wishImage, MonoBehaviour owner)
        {
            _wishImage = wishImage;
            _owner = owner;
        }

        public void Initialize(EmoteContainer emoteContainer)
        {
            EmoteContainer = emoteContainer;

            if (emoteContainer == null || _emoteCache != null) return;

            _emoteCache = new Dictionary<EmoteType, Sprite>(emoteContainer.Emotes.Length);
            foreach (Emote emote in emoteContainer.Emotes)
                _emoteCache[emote.Type] = emote.Sprite;
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

        public void SetSprite(Sprite sprite)
        {
            if (_wishImage == null) return;
            StopDismiss();
            _wishImage.color = ColorExtensions.Visible();
            _wishImage.sprite = sprite;
            PlayPopIn();
        }

        public void ClearSprite()
        {
            if (_wishImage == null) return;
            PlayDismiss(() =>
            {
                _wishImage.sprite = null;
                _wishImage.color = ColorExtensions.Transparent;
            });
        }

        public void SetEmote(EmoteType type)
        {
            if (_wishImage == null) return;
            if (_emoteCache == null || !_emoteCache.TryGetValue(type, out Sprite sprite)) return;

            StopDismiss();
            _wishImage.sprite = sprite;
            _wishImage.color = ColorExtensions.Visible();
            PlayPopIn();
        }

        public void ClearEmote()
        {
            if (_wishImage == null) return;
            PlayDismiss(() =>
            {
                _wishImage.sprite = null;
                _wishImage.color = ColorExtensions.Transparent;
            });
        }

        // ── Pop-in ──────────────────────────────────────────────────────────────

        private void PlayPopIn()
        {
            if (_popRoutine != null)
                _owner.StopCoroutine(_popRoutine);

            _popRoutine = _owner.StartCoroutine(PopInRoutine());
        }

        private IEnumerator PopInRoutine()
        {
            Transform t = _wishImage.rectTransform;
            t.localScale = Vector3.zero;

            // Фаза 1: 0 → OvershootScale
            float phase1 = PopDuration * 0.6f;
            float elapsed = 0f;

            while (elapsed < phase1)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.LerpUnclamped(0f, OvershootScale, EaseOutCubic(elapsed / phase1));
                t.localScale = Vector3.one * scale;
                yield return null;
            }

            // Фаза 2: OvershootScale → FinalScale
            float phase2 = PopDuration * 0.4f;
            elapsed = 0f;

            while (elapsed < phase2)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.LerpUnclamped(OvershootScale, FinalScale, EaseOutCubic(elapsed / phase2));
                t.localScale = Vector3.one * scale;
                yield return null;
            }

            t.localScale = Vector3.one * FinalScale;
            _popRoutine = null;
        }

        // ── Dismiss ─────────────────────────────────────────────────────────────

        private void PlayDismiss(System.Action onComplete)
        {
            if (_dismissRoutine != null)
                _owner.StopCoroutine(_dismissRoutine);

            if (_popRoutine != null)
            {
                _owner.StopCoroutine(_popRoutine);
                _popRoutine = null;
            }

            _dismissRoutine = _owner.StartCoroutine(DismissRoutine(onComplete));
        }

        private void StopDismiss()
        {
            if (_dismissRoutine == null) return;
            _owner.StopCoroutine(_dismissRoutine);
            _dismissRoutine = null;

            // Сбрасываем альфу и масштаб если dismiss был прерван новым SetEmote
            if (_wishImage != null)
            {
                Color c = _wishImage.color;
                c.a = 1f;
                _wishImage.color = c;
                _wishImage.rectTransform.localScale = Vector3.one * FinalScale;
            }
        }

        private IEnumerator DismissRoutine(System.Action onComplete)
        {
            Transform t = _wishImage.rectTransform;
            Color startColor = _wishImage.color;
            float startScale = t.localScale.x;

            float elapsed = 0f;

            while (elapsed < DismissDuration)
            {
                elapsed += Time.deltaTime;
                float raw = EaseInCubic(elapsed / DismissDuration);

                // Сжимаемся и исчезаем одновременно
                float scale = Mathf.LerpUnclamped(startScale, DismissShrinkScale, raw);
                float alpha = Mathf.LerpUnclamped(1f, 0f, raw);

                t.localScale = Vector3.one * scale;

                Color c = startColor;
                c.a = alpha;
                _wishImage.color = c;

                yield return null;
            }

            t.localScale = Vector3.one * FinalScale; // сбрасываем масштаб для следующего появления
            onComplete?.Invoke();
            _dismissRoutine = null;
        }

        // ── Easing ──────────────────────────────────────────────────────────────

        private static float EaseOutCubic(float t) =>
            1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);

        private static float EaseInCubic(float t) =>
            Mathf.Pow(Mathf.Clamp01(t), 3f);
    }
}