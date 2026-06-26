using Specs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace Core.Dialogue
{
    public sealed class DialogueSystem : MonoBehaviour
    {
        private int _dialogueIndex = -1;
        [SerializeField] private Button _skipDialogueBtn = null;
        [SerializeField] private MoodDialogueEntry[] _moodEntries;
        private IDialogueVisualizer _visualizer = null;
        private Dictionary<DialogueMood, DialogueTree[]> _moodTrees;
        private DialogueTree _currentTree = null;

        public static DialogueSystem Instance = null;

        private void Awake()
        {
            InitializeSingleton();
            BuildMoodDictionary();

            if (_visualizer == null)
            {
                _visualizer = GetComponentInChildren<IDialogueVisualizer>();
            }
        }

        private void Start()
        {
            if (_skipDialogueBtn == null) _skipDialogueBtn = GetComponentInChildren<Button>();

            _skipDialogueBtn.onClick.AddListener(NextNode);
        }

        private void BuildMoodDictionary()
        {
            _moodTrees = new Dictionary<DialogueMood, DialogueTree[]>(_moodEntries.Length);
            foreach (var entry in _moodEntries)
                _moodTrees[entry.Mood] = entry.Trees;
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

            _visualizer.Visualize(_currentTree.GetNode(_dialogueIndex), sprite);
        }

        public void EndDialogue()
        {
            _currentTree = null;
            _dialogueIndex = 0;
            _visualizer.ClearText();
        }

        public void RegisterInput(InputAction.CallbackContext context)
        {
            NextNode();
        }

        private void NextNode()
        {
            if (_currentTree == null) return;

            if (_visualizer.IsRevealing())
            {
                _visualizer.SkipReveal();
                return;
            }

            _dialogueIndex++;
            DialogueNode newNode = _currentTree.GetNode(_dialogueIndex);

            if (newNode == null)
            {
                EndDialogue();
                return;
            }

            _visualizer.Visualize(newNode);
        }


        private void OnDestroy()
        {
            _skipDialogueBtn.onClick.RemoveListener(NextNode);
        }
    }
}
