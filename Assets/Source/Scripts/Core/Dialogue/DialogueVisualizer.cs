using Specs;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Dialogue
{
    public sealed class DialogueVisualizer : MonoBehaviour, IDialogueVisualizer
    {
        [SerializeField] private float _textSpeed = 0.1f;
        [SerializeField] private TextMeshProUGUI _tmp = null;
        [SerializeField] private Image _icon = null;

        private DialogueNode _currentNode = null;
        private Coroutine _revealCoroutine = null;
        private bool _isRevealing = false;

        private void Awake()
        {
            if (_tmp == null)
            {
                _tmp = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        public void Visualize(DialogueNode node, Sprite sprite = null)
        {
            _currentNode = node;

            if (_currentNode == null)
            {
                ClearText();
                return;
            }

            StopRevealCoroutine();

            if (sprite != null)
            {
                _icon.color = Color.black; 
                _icon.sprite = sprite;
            }

            _revealCoroutine = StartCoroutine(RevealText(_currentNode.NpcText));
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

        public void ClearText()
        {
            StopRevealCoroutine();
            _icon.color = Color.clear; 
            _tmp.text = "";
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
        }
    }
}