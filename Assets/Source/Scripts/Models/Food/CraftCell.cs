using Core.Food;
using DG.Tweening;
using Specs;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Models.Food
{
    public class CraftCell : MonoBehaviour, IUICell<IngredientItem>, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] protected Image _image = null;
        public bool IsEmpty { get; set; }

        protected IngredientItem _currentIngredient = null;

        private Color _transparent = new Color(1, 1, 1, 0);
        private Color _visible = new Color(1, 1, 1, 1);

        private static Image _dragIcon;
        private static RectTransform _dragRectTransform;
        private static Canvas _canvas;
        private static Vector3 _originalPosition;
        private static Transform _originalCell;
        private CraftCell _dragSourceCell;

        private void Awake()
        {
            Debug.Log($"[CraftCell] Awake on {gameObject.name}");

            if (_image == null)
            {
                _image = GetComponentInChildren<Image>();
                Debug.Log($"[CraftCell] _image auto-assigned: {(_image != null)}");
            }

            if (_canvas == null)
            {
                _canvas = GetComponentInParent<Canvas>();
                Debug.Log($"[CraftCell] _canvas found: {(_canvas != null)}, name: {_canvas?.name}");
            }
        }

        protected virtual void Start()
        {
            Debug.Log($"[CraftCell] Start on {gameObject.name}, registering with CraftPlate");
            CraftPlate.Instance.RegisterCraftingCell(this);
        }

        public IngredientItem GetItem()
        {
            return _currentIngredient;
        }

        public virtual void SetItem(IngredientItem ingredient)
        {
            Debug.Log($"[CraftCell] SetItem on {gameObject.name}, ingredient: {(ingredient != null ? ingredient.name : "null")}");
            _currentIngredient = ingredient;
            ConfigureIngredientInCell();
            IsEmpty = ingredient == null;
        }

        protected virtual void ConfigureIngredientInCell()
        {
            Debug.Log($"[CraftCell] ConfigureIngredientInCell on {gameObject.name}, _currentIngredient: {(_currentIngredient != null ? _currentIngredient.name : "null")}");

            if (_currentIngredient == null)
            {
                _image.color = _transparent;
                Debug.Log($"[CraftCell] Setting transparent color");
                return;
            }

            _image.color = _visible;
            _image.sprite = _currentIngredient.Icon;
            Debug.Log($"[CraftCell] Setting visible color and sprite: {_currentIngredient.Icon?.name}");
        }

        public void ClearCell()
        {
            Debug.Log($"[CraftCell] ClearCell on {gameObject.name}");
            _currentIngredient = null;
            _image.color = _transparent;
            _image.sprite = null;
            IsEmpty = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
           

            if (IsEmpty || _currentIngredient == null)
            {
                Debug.LogWarning($"[CraftCell] OnBeginDrag BLOCKED - cell is empty or ingredient is null");
                return;
            }

            _dragSourceCell = this;

            // Create global drag icon
            if (_dragIcon == null)
            {
                Debug.Log("[CraftCell] Creating new global drag icon");
                var dragObject = new GameObject("GlobalDragIcon");
                _dragIcon = dragObject.AddComponent<Image>();
                _dragRectTransform = dragObject.GetComponent<RectTransform>();
                _dragIcon.raycastTarget = false;

                if (_canvas != null)
                {
                    _dragRectTransform.SetParent(_canvas.transform);
                    _dragRectTransform.SetAsLastSibling();
                    Debug.Log($"[CraftCell] Drag icon parented to canvas: {_canvas.name}");
                }
                else
                {
                    Debug.LogError("[CraftCell] CANVAS IS NULL! Drag icon won't be parented properly!");
                }
            }

            _dragIcon.sprite = _currentIngredient.Icon;
            _dragIcon.SetNativeSize();
            _dragIcon.gameObject.SetActive(true);


            SetDragPosition(eventData);

            // Animate appearance
            Debug.Log("[CraftCell] Starting drag icon animation");
            _dragRectTransform.localScale = Vector3.zero;
            _dragRectTransform.DOScale(5f, .2f).SetEase(Ease.OutBack);

            // Hide original icon
            _image.color = _transparent;
            Debug.Log("[CraftCell] Original image hidden");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragIcon == null || !_dragIcon.gameObject.activeSelf)
            {
                if (_dragIcon == null)
                    Debug.LogWarning("[CraftCell] OnDrag - _dragIcon is null");
                else
                    Debug.LogWarning("[CraftCell] OnDrag - drag icon not active");
                return;
            }

            SetDragPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {

            if (_dragIcon == null || !_dragIcon.gameObject.activeSelf || _dragSourceCell == null)
            {
                return;
            }

            // Find target cell
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            Debug.Log($"[CraftCell] Raycast results count: {results.Count}");

            CraftCell targetCell = null;

            foreach (var result in results)
            {
                Debug.Log($"[CraftCell] Raycast hit: {result.gameObject.name}");
                targetCell = result.gameObject.GetComponent<CraftCell>();
                if (targetCell != null)
                {
                    break;
                }
            }

            if (targetCell != null && targetCell != _dragSourceCell)
            {

                var sourceIngredient = _dragSourceCell._currentIngredient;
                var targetIngredient = targetCell._currentIngredient;
                bool targetWasEmpty = targetCell.IsEmpty;


                // Animate drop
                _dragRectTransform.DOMove(targetCell.transform.position, 0.2f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        _dragIcon.gameObject.SetActive(false);

                        // Perform the swap
                        if (targetWasEmpty)
                        {
                            targetCell.SetItem(sourceIngredient);
                            _dragSourceCell.ClearCell();
                        }
                        else
                        {
                            Debug.Log("[CraftCell] Swapping ingredients");
                            targetCell.SetItem(sourceIngredient);
                            _dragSourceCell.SetItem(targetIngredient);
                        }

                        // Return drag icon to original position for potential reuse
                        _dragRectTransform.position = _dragSourceCell.transform.position;
                    });

                _dragRectTransform.DOScale(0.5f, 0.2f);
            }
            else
            {

                // Return to original cell with animation
                _dragRectTransform.DOMove(_dragSourceCell.transform.position, 0.3f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        Debug.Log("[CraftCell] Return animation complete");
                        _dragIcon.gameObject.SetActive(false);
                        _dragSourceCell.ConfigureIngredientInCell();
                        _dragRectTransform.position = _dragSourceCell.transform.position;
                    });
                _dragRectTransform.DOScale(4, 0.4f);
            }
        }

        private void SetDragPosition(PointerEventData eventData)
        {
            if (_dragRectTransform == null || _canvas == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            _dragRectTransform.localPosition = localPoint;
        }

        private void OnDestroy()
        {
            if (_dragIcon != null && !this.IsDestroyed())
            {
                Destroy(_dragIcon.gameObject);
            }
        }
    }
}
 