using Core;
using DG.Tweening;
using Specs;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Models
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
            if (_image == null)
                _image = GetComponentInChildren<Image>();

            if (_canvas == null)
                _canvas = GetComponentInParent<Canvas>();
        }

        protected virtual void Start()
        {
            CraftPlate.Instance.RegisterCraftingCell(this);
        }

        public IngredientItem GetItem()
        {
            return _currentIngredient;
        }

        public virtual void SetItem(IngredientItem ingredient)
        {
            _currentIngredient = ingredient;
            ConfigureIngredientInCell();
            IsEmpty = ingredient == null; // ← фикс
        }

        protected virtual void ConfigureIngredientInCell()
        {
            if (_currentIngredient == null)
            {
                _image.color = _transparent;
                return;
            }

            _image.color = _visible;
            _image.sprite = _currentIngredient.Icon;
        }

        public void ClearCell()
        {
            _currentIngredient = null;
            _image.color = _transparent;
            _image.sprite = null;
            IsEmpty = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsEmpty || _currentIngredient == null)
                return;

            _dragSourceCell = this;

            // Create global drag icon
            if (_dragIcon == null)
            {
                var dragObject = new GameObject("GlobalDragIcon");
                _dragIcon = dragObject.AddComponent<Image>();
                _dragRectTransform = dragObject.GetComponent<RectTransform>();
                _dragIcon.raycastTarget = false;

                if (_canvas != null)
                {
                    _dragRectTransform.SetParent(_canvas.transform);
                    _dragRectTransform.SetAsLastSibling();
                }
            }

            _dragIcon.sprite = _currentIngredient.Icon;
            _dragIcon.SetNativeSize();
            _dragIcon.gameObject.SetActive(true);

            SetDragPosition(eventData);

            // Animate appearance
            _dragRectTransform.localScale = Vector3.zero;
            _dragRectTransform.DOScale(5f, .2f).SetEase(Ease.OutBack);

            // Hide original icon
            _image.color = _transparent;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragIcon == null || !_dragIcon.gameObject.activeSelf)
                return;

            SetDragPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragIcon == null || !_dragIcon.gameObject.activeSelf || _dragSourceCell == null)
                return;

            // Find target cell
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            CraftCell targetCell = null;

            foreach (var result in results)
            {
                targetCell = result.gameObject.GetComponent<CraftCell>();
                if (targetCell != null)
                    break;
            }

            if (targetCell != null && targetCell != _dragSourceCell)
            {
                // Swap ingredients between cells
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
                return;

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
                Destroy(_dragIcon.gameObject);
        }
    }
}