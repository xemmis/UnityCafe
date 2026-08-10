using DG.Tweening;
using Specs;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.Dialogue
{
    public sealed class DialogueVisualizer : MonoBehaviour, IDialogueVisualizer
    {
        [SerializeField] private float _textSpeed = 0.1f;
        [SerializeField] private TextMeshProUGUI _tmp = null;
        [SerializeField] private Image _icon = null;

        [Header("Popup Movement")]
        [SerializeField] private Transform _popup = null;
        [SerializeField] private Transform _openPos = null;
        [SerializeField] private Transform _closePos = null;
        [SerializeField] private float _moveDuration = 0.5f;

        private DialogueNode _currentNode = null;
        private Coroutine _revealCoroutine = null;
        private bool _isRevealing = false;
        private Tween _moveTween = null;

        private void Awake()
        {
            if (_tmp == null)
                _tmp = GetComponentInChildren<TextMeshProUGUI>();

            if (_icon == null)
                _icon = GetComponentInChildren<Image>();

            if (_popup == null)
                Debug.LogError($"[DialogueVisualizer] _popup не назначен на {gameObject.name}!", this);

            if (_openPos == null || _closePos == null)
                Debug.LogError($"[DialogueVisualizer] _openPos/_closePos не назначены на {gameObject.name}!", this);
        }

        public void Open(DialogueNode node, Sprite sprite = null)
        {
            MoveTo(_openPos);
            ShowNode(node, sprite);
        }

        public void ShowNode(DialogueNode node, Sprite sprite = null)
        {
            _currentNode = node;

            if (_currentNode == null)
            {
                ClearText();
                return;
            }

            StopRevealCoroutine();

            if (sprite != null && _icon != null)
            {
                _icon.color = ColorExtensions.Visible;
                _icon.sprite = sprite;
            }

            _revealCoroutine = StartCoroutine(RevealText(_currentNode.NpcText));
        }

        public void Close()
        {
            ClearText();
            MoveTo(_closePos);
        }

        private void MoveTo(Transform target)
        {
            if (_popup == null || target == null) return;

            _moveTween?.Kill();
            _moveTween = _popup.DOMove(target.position, _moveDuration).SetEase(Ease.OutQuad);
        }

        private IEnumerator RevealText(string fullText)
        {
            _isRevealing = true;
            _tmp.text = "";

            for (int i = 0; i < fullText.Length; i++)
            {
                _tmp.text += fullText[i];
                yield return new WaitForSeconds(_textSpeed);
            }

            _isRevealing = false;
            _revealCoroutine = null;
        }

        public bool IsRevealing() => _isRevealing;

        public void SkipReveal()
        {
            if (_isRevealing && _currentNode != null)
            {
                StopRevealCoroutine();
                _tmp.text = _currentNode.NpcText;
                _isRevealing = false;
            }
        }

        private void ClearText()
        {
            StopRevealCoroutine();
            if (_icon != null) _icon.color = ColorExtensions.Transparent;
            if (_tmp != null) _tmp.text = "";
            _currentNode = null;
        }

        private void StopRevealCoroutine()
        {
            if (_revealCoroutine != null)
            {
                StopCoroutine(_revealCoroutine);
                _revealCoroutine = null;
                _isRevealing = false;
            }
        }

        private void OnDestroy()
        {
            StopRevealCoroutine();
            _moveTween?.Kill();
        }
    }
}