using Specs;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Dialogue
{
    public sealed class DialogueSystem : MonoBehaviour
    {
        private int _dialogueIndex = -1;
        private bool _isDialogueActive = false;

        private IDialogueVisualizer _visualizer = null;
        [SerializeField] private MoodDialogueEntry[] _moodEntries;
        private Dictionary<DialogueMood, DialogueTree[]> _moodTrees;
        private DialogueTree _currentTree = null;

        public static DialogueSystem Instance = null;

        private void Awake()
        {
            InitializeSingleton();
            BuildMoodDictionary();

            if (_visualizer == null)
                _visualizer = GetComponentInChildren<IDialogueVisualizer>();

            if (_visualizer == null)
                Debug.LogError("[DialogueSystem] Не найден компонент IDialogueVisualizer среди дочерних объектов!", this);
        }

        private void BuildMoodDictionary()
        {
            _moodTrees = new Dictionary<DialogueMood, DialogueTree[]>(_moodEntries.Length);
            foreach (var entry in _moodEntries)
                _moodTrees[entry.Mood] = entry.Trees;
        }

        private void InitializeSingleton()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void StartDialogue(DialogueMood mood, Sprite sprite = null)
        {
            if (!_moodTrees.TryGetValue(mood, out DialogueTree[] trees) || trees.Length == 0)
            {
                Debug.LogWarning($"[DialogueSystem] No trees for mood: {mood}");
                return;
            }

            DialogueTree tree = trees[Random.Range(0, trees.Length)];
            StartDialogue(tree, sprite);
        }

        public void StartDialogue(DialogueTree dialogueTree, Sprite sprite = null)
        {
            if (dialogueTree == null || _visualizer == null) return;

            _currentTree = dialogueTree;
            _dialogueIndex = 0;
            _isDialogueActive = true;

            _visualizer.Open(_currentTree.GetNode(_dialogueIndex), sprite);
        }

        public void OnNextButtonPressed()
        {
            if (!_isDialogueActive || _visualizer == null) return;

            if (_visualizer.IsRevealing())
            {
                _visualizer.SkipReveal();
                return;
            }

            _dialogueIndex++;
            DialogueNode newNode = _currentTree?.GetNode(_dialogueIndex);

            if (newNode == null)
            {
                EndDialogue();
                return;
            }

            _visualizer.ShowNode(newNode);
        }

        public void EndDialogue()
        {
            _isDialogueActive = false;
            _currentTree = null;
            _dialogueIndex = 0;
            _visualizer?.Close();
        }
    }
}