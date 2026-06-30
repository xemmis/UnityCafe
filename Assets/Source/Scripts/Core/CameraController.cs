using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace Core
{
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputAsset;
        [SerializeField] private float panSpeed = 5f;
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private float minX = -10f;
        [SerializeField] private float maxX = 10f;

        private InputAction _pressAction;
        private InputAction _pointerAction;
        private InputAction _keyboardAction;
        private Vector2 _lastPointerPosition;
        private bool _isDragging;
        private float _targetX;
        private Tween _moveTween;

        private void Awake()
        {
            _pressAction = _inputAsset.FindAction("Camera/Press");
            _pointerAction = _inputAsset.FindAction("Camera/PointerPosition");
            _keyboardAction = _inputAsset.FindAction("Camera/Keyboard");

            _pressAction.performed += OnPress;
            _pressAction.canceled += OnRelease;
            _pointerAction.performed += OnPointerMove;
            _keyboardAction.performed += OnKeyboard;
            _keyboardAction.canceled += OnKeyboardCanceled;


        }

        private void Start()
        {
            _targetX = transform.position.x;
            EnableMovement();
            GameCondition.OnCameraConditionChanged += HandleCameraCondition;
        }

        private void HandleCameraCondition(bool condition)
        {
            if (condition) EnableMovement();
            else DisableMovement();
        }

        private void OnDestroy()
        {
            _pressAction.performed -= OnPress;
            _pressAction.canceled -= OnRelease;
            _pointerAction.performed -= OnPointerMove;
            _keyboardAction.performed -= OnKeyboard;
            _keyboardAction.canceled -= OnKeyboardCanceled;
            GameCondition.OnCameraConditionChanged -= HandleCameraCondition;
            _moveTween?.Kill();
        }

        public void EnableMovement()
        {
            _pressAction.Enable();
            _pointerAction.Enable();
            _keyboardAction.Enable();
        }

        public void DisableMovement()
        {
            _pressAction.Disable();
            _pointerAction.Disable();
            _keyboardAction.Disable();

            _isDragging = false;
            _moveTween?.Kill();
        }

        private void OnPress(InputAction.CallbackContext context)
        {
            _isDragging = true;
            _moveTween?.Kill();
            _lastPointerPosition = _pointerAction.ReadValue<Vector2>();
        }

        private void OnRelease(InputAction.CallbackContext context)
        {
            _isDragging = false;
        }

        private void OnPointerMove(InputAction.CallbackContext context)
        {
            if (!_isDragging) return;

            Vector2 delta = context.ReadValue<Vector2>() - _lastPointerPosition;
            _targetX = Mathf.Clamp(transform.position.x - delta.x * panSpeed * Time.deltaTime, minX, maxX);
            transform.position = new Vector3(_targetX, transform.position.y, transform.position.z);
            _lastPointerPosition = context.ReadValue<Vector2>();
        }

        private void OnKeyboard(InputAction.CallbackContext context)
        {
            _moveTween?.Kill();
            float input = context.ReadValue<Vector2>().x;
            _targetX = Mathf.Clamp(transform.position.x + input * 100f, minX, maxX);

            _moveTween = transform.DOMoveX(_targetX, smoothTime).SetEase(Ease.OutQuad);
        }

        private void OnKeyboardCanceled(InputAction.CallbackContext context)
        {
            _moveTween?.Kill();
        }
    }
}