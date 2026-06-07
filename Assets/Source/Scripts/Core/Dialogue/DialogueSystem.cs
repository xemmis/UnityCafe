using Specs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Core.Dialogue
{
    public sealed class DialogueSystem : MonoBehaviour
    {
        private int _dialogueIndex = -1;

        private IDialogueVisualizer _visualizer = null;
        [SerializeField] private MoodDialogueEntry[] _moodEntries;
        private Dictionary<DialogueMood, DialogueTree[]> _moodTrees;
        private DialogueTree _currentTree = null;
        private InputActionAsset _uiActionAsset = null;
        private InputAction _clickAction = null;

        public static DialogueSystem Instance = null;

        private void Awake()
        {
            InitializeSingleton();
            InitializeInput();
            BuildMoodDictionary();

            if (_visualizer == null)
            {
                _visualizer = GetComponentInChildren<IDialogueVisualizer>();
            }
        }

        private void BuildMoodDictionary()
        {
            _moodTrees = new Dictionary<DialogueMood, DialogueTree[]>(_moodEntries.Length);
            foreach (var entry in _moodEntries)
                _moodTrees[entry.Mood] = entry.Trees;
        }

        private void InitializeInput()
        {
            if (_uiActionAsset == null)
                return;

            _clickAction = _uiActionAsset.FindAction("Click");
        }

        private void InitializeSingleton()
        {
            if (DialogueSystem.Instance == null)
            {
                DialogueSystem.Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void StartDialogue(DialogueMood mood, Sprite sprite = null)
        {
            if (!_moodTrees.TryGetValue(mood, out DialogueTree[] trees) || trees.Length == 0)
            {
                Debug.LogWarning($"[DialogueSystem] No trees for mood: {mood}");
                return;
            }

            DialogueTree tree = trees[UnityEngine.Random.Range(0, trees.Length)];
            StartDialogue(tree, sprite);
        }

        public void StartDialogue(DialogueTree dialogueTree, Sprite sprite = null)
        {
            _currentTree = dialogueTree;
            _dialogueIndex = 0;
            _clickAction.performed += RegisterInput;

            _visualizer.Visualize(_currentTree.GetNode(_dialogueIndex), sprite);
        }

        public void RegisterInput(InputAction.CallbackContext context)
        {
            NextNode();
        }

        private void NextNode()
        {
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
            }

            _visualizer.Visualize(newNode);
        }

        public void EndDialogue()
        {
            _currentTree = null;
            _dialogueIndex = 0;
            _visualizer.ClearText();
            _clickAction.performed -= RegisterInput;
        }
    }
}
