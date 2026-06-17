using UnityEngine;
using UnityEngine.InputSystem;

namespace Models.Plant
{
    public sealed class WishDragController : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private InputActionAsset _inputActions;

        private InputAction _pointerDown;
        private InputAction _pointerDrag;

        private WishExecuter _currentWish;
        private bool _isDragging;
        private Vector3 _offset;
        private float _zDistance;

        private void Awake()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            var plantMap = _inputActions.FindActionMap("Plant");
            _pointerDown = plantMap.FindAction("PointerDown");
            _pointerDrag = plantMap.FindAction("PointerDrag");
        }

        private void OnEnable()
        {
            _pointerDown.started += OnPointerDown;
            _pointerDown.canceled += OnPointerUp;
            _pointerDrag.performed += OnPointerDrag;
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _pointerDown.started -= OnPointerDown;
            _pointerDown.canceled -= OnPointerUp;
            _pointerDrag.performed -= OnPointerDrag;
            _inputActions?.Disable();
            _isDragging = false;
            _currentWish = null;
        }

        private void OnPointerDown(InputAction.CallbackContext context)
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(_mainCamera.ScreenPointToRay(screenPos));
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent<WishExecuter>(out WishExecuter wish))
                {
                    if (!wish.CanDrag) continue;

                    if (_currentWish != null && _currentWish != wish)
                    {
                        _currentWish.ReturnToStart();
                        _currentWish.OnDragEnd();
                    }

                    _currentWish = wish;
                    _isDragging = true;
                    _zDistance = _mainCamera.WorldToScreenPoint(wish.transform.position).z;
                    _offset = wish.transform.position - GetWorldPoint(screenPos);

                    wish.OnDragStart();
                    break;
                }
            }
        }

        private void OnPointerDrag(InputAction.CallbackContext context)
        {
            if (!_isDragging || _currentWish == null) return;

            Vector2 screenPosition = context.ReadValue<Vector2>();
            Vector3 targetPosition = GetWorldPoint(screenPosition) + _offset;

            _currentWish.MoveTo(targetPosition);
        }

        private void OnPointerUp(InputAction.CallbackContext context)
        {
            if (!_isDragging || _currentWish == null) return;

            _isDragging = false;

            Collider2D hit = Physics2D.OverlapPoint(_currentWish.transform.position);

            if (hit != null && hit.TryGetComponent<PlantBehavior>(out PlantBehavior plant))
            {
                _currentWish.Execute(plant);
            }

            _currentWish.ReturnToStart();
            _currentWish.OnDragEnd();
            _currentWish = null;
        }

        private Vector3 GetWorldPoint(Vector2 screenPosition)
        {
            Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, _zDistance);
            return _mainCamera.ScreenToWorldPoint(screenPoint);
        }
    }
}