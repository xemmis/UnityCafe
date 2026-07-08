using Core;
using DG.Tweening;
using Models.Npc;
using Specs;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Models.Food
{
    public class FoodItemCell : MonoBehaviour, IUICell<FoodItem>, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] protected Image _image = null;
        public bool IsEmpty { get; set; } = true;

        protected FoodItem _currentFood = null;

        private Color _transparent = new Color(1, 1, 1, 0);
        private Color _visible = new Color(1, 1, 1, 1);

        private static Image _dragIcon;
        private static RectTransform _dragRectTransform;
        private static Canvas _canvas;
        private FoodItemCell _dragSourceCell;

        private void Awake()
        {
            if (_image == null)
                _image = GetComponentInChildren<Image>();

            if (_canvas == null)
                _canvas = GetComponentInParent<Canvas>();
        }

        public FoodItem GetItem()
        {
            return _currentFood;
        }

        public virtual void SetItem(FoodItem item)
        {
            _currentFood = item;
            ConfigureFoodInCell();
            IsEmpty = item == null;
        }

        protected virtual void ConfigureFoodInCell()
        {
            if (_currentFood == null)
            {
                _image.color = _transparent;
                _image.sprite = null;
                return;
            }

            _image.color = _visible;
            _image.sprite = _currentFood.Icon;
        }

        public void ClearCell()
        {
            _currentFood = null;
            _image.color = _transparent;
            _image.sprite = null;
            IsEmpty = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsEmpty || _currentFood == null)
                return;
            GameCondition.ChangeCameraControllCondition(false);
            _dragSourceCell = this;

            // Create global drag icon
            if (_dragIcon == null)
            {
                var dragObject = new GameObject("GlobalFoodDragIcon");
                _dragIcon = dragObject.AddComponent<Image>();
                _dragRectTransform = dragObject.GetComponent<RectTransform>();
                _dragIcon.raycastTarget = false;

                if (_canvas != null)
                {
                    _dragRectTransform.SetParent(_canvas.transform);
                    _dragRectTransform.SetAsLastSibling();
                }
            }

            _dragIcon.sprite = _currentFood.Icon;
            _dragIcon.SetNativeSize();
            _dragIcon.gameObject.SetActive(true);

            SetDragPosition(eventData);

            // Animate appearance
            _dragRectTransform.localScale = Vector3.zero;
            _dragRectTransform.DOScale(5f, 0.2f).SetEase(Ease.OutBack);

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
            GameCondition.ChangeCameraControllCondition(true);

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0f;

            // Ищем NPC через Physics2D overlap
            Collider2D hit = Physics2D.OverlapPoint(worldPos);
            NpcInteraction targetNpc = hit?.GetComponent<NpcInteraction>();
            
            // UI ячейки ищем как раньше через RaycastAll
            FoodItemCell targetCell = null;
            if (targetNpc == null)
            {
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                foreach (var result in results)
                {
                    var cell = result.gameObject.GetComponent<FoodItemCell>();
                    if (cell != null && cell != _dragSourceCell)
                    {
                        targetCell = cell;
                        break;
                    }
                }
            }

            if (targetNpc != null && targetNpc.IsWaitingFood)
            {
                // Конвертируем мировую позицию NPC в позицию на Canvas
                Vector2 screenPos = Camera.main.WorldToScreenPoint(targetNpc.transform.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.GetComponent<RectTransform>(),
                    screenPos,
                    null,
                    out Vector2 canvasPos
                );

                _dragRectTransform.DOLocalMove(canvasPos, 0.2f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        _dragIcon.gameObject.SetActive(false);
                        HandleDropOnNpc(targetNpc, _dragSourceCell._currentFood);
                        _dragSourceCell.ClearCell();
                    });

                _dragRectTransform.DOScale(0.5f, 0.2f);
            }

            else if (targetCell != null)
            {
                // Dropped on another FoodItemCell - swap
                var sourceFood = _dragSourceCell._currentFood;
                var targetFood = targetCell._currentFood;
                bool targetWasEmpty = targetCell.IsEmpty;

                _dragRectTransform.DOMove(targetCell.transform.position, 0.2f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        _dragIcon.gameObject.SetActive(false);

                        // Perform the swap
                        if (targetWasEmpty)
                        {
                            targetCell.SetItem(sourceFood);
                            _dragSourceCell.ClearCell();
                        }
                        else
                        {
                            targetCell.SetItem(sourceFood);
                            _dragSourceCell.SetItem(targetFood);
                        }

                        _dragRectTransform.position = _dragSourceCell.transform.position;
                    });

                _dragRectTransform.DOScale(0.5f, 0.2f);
            }
            else
            {
                // Dropped on invalid target - return to original position
                _dragRectTransform.DOScale(1.5f, 0.4f);
                _dragRectTransform.DOMove(_dragSourceCell.transform.position, 0.3f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        _dragIcon.gameObject.SetActive(false);
                        _dragSourceCell.ConfigureFoodInCell();
                        _dragRectTransform.position = _dragSourceCell.transform.position;
                    });
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

        // Метод для обработки дропа на NPC - здесь будет ваша логика
        protected virtual void HandleDropOnNpc(NpcInteraction npc, FoodItem food)
        {
            // Базовая заглушка - вы переопределите этот метод или замените логикой
            Debug.Log($"Dropped {food.FoodName} on {npc.name}");
            npc.AcceptFood(food);
            // npc.ReceiveFood(food); // пример вызова
        }

        private void OnDestroy()
        {
            if (_dragIcon != null && !this.IsDestroyed())
                Destroy(_dragIcon.gameObject);
        }
    }
}