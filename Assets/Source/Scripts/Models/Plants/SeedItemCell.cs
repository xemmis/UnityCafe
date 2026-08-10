using Core;
using DG.Tweening;
using Specs;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Models.Plant
{
    public sealed class SeedItemCell : MonoBehaviour, IUICell<PlantSO>, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _image = null;
        [SerializeField] private TextMeshProUGUI _amountText = null;
        public bool IsEmpty { get; set; } = true;

        private PlantSO _currentSeed = null;
        private int _amount = 0;

        private Color _transparent = new Color(1, 1, 1, 0);
        private Color _visible = new Color(1, 1, 1, 1);

        private static Image _dragIcon;
        private static RectTransform _dragRectTransform;
        private static Canvas _canvas;
        private SeedItemCell _dragSourceCell;

        private void Awake()
        {
            if (_image == null)
                _image = GetComponentInChildren<Image>();

            if (_canvas == null)
                _canvas = GetComponentInParent<Canvas>();
        }

        public PlantSO GetItem() => _currentSeed;

        public void SetItem(PlantSO seed) => SetItem(seed, SeedInventory.GetAmount(seed));

        public void SetItem(PlantSO seed, int amount)
        {
            _currentSeed = seed;
            _amount = amount;
            ConfigureSeedInCell();
            IsEmpty = seed == null || amount <= 0;
        }

        private void ConfigureSeedInCell()
        {
            if (_currentSeed == null || _amount <= 0)
            {
                _image.color = _transparent;
                _image.sprite = null;
                if (_amountText != null) _amountText.text = "";
                return;
            }

            _image.color = _visible;
            _image.sprite = _currentSeed.Ingredinet != null ? _currentSeed.Ingredinet.Icon : null;
            if (_amountText != null) _amountText.text = _amount.ToString();
        }

        public void ClearCell()
        {
            _currentSeed = null;
            _amount = 0;
            _image.color = _transparent;
            _image.sprite = null;
            if (_amountText != null) _amountText.text = "";
            IsEmpty = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsEmpty || _currentSeed == null) return;

            GameCondition.ChangeCameraControllCondition(false);
            _dragSourceCell = this;

            if (_dragIcon == null)
            {
                var dragObject = new GameObject("GlobalSeedDragIcon");
                _dragIcon = dragObject.AddComponent<Image>();
                _dragRectTransform = dragObject.GetComponent<RectTransform>();
                _dragIcon.raycastTarget = false;

                if (_canvas != null)
                {
                    _dragRectTransform.SetParent(_canvas.transform);
                    _dragRectTransform.SetAsLastSibling();
                }
            }

            _dragIcon.sprite = _image.sprite;
            _dragIcon.SetNativeSize();
            _dragIcon.gameObject.SetActive(true);

            SetDragPosition(eventData);

            _dragRectTransform.localScale = Vector3.zero;
            _dragRectTransform.DOScale(5f, 0.2f).SetEase(Ease.OutBack);

            _image.color = _transparent;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragIcon == null || !_dragIcon.gameObject.activeSelf) return;
            SetDragPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragIcon == null || !_dragIcon.gameObject.activeSelf || _dragSourceCell == null)
                return;

            GameCondition.ChangeCameraControllCondition(true);

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0f;

            Collider2D hit = Physics2D.OverlapPoint(worldPos);
            PlantBehavior targetPlant = hit != null ? hit.GetComponent<PlantBehavior>() : null;

            if (targetPlant != null && targetPlant.PlantData == null)
            {
                PlantSO seedToPlant = _dragSourceCell._currentSeed;

                _dragRectTransform.DOMove(targetPlant.transform.position, 0.2f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        _dragIcon.gameObject.SetActive(false);
                        targetPlant.Initialize(seedToPlant);
                        SeedInventory.Remove(seedToPlant);
                    });

                _dragRectTransform.DOScale(0.5f, 0.2f);
            }
            else
            {
                _dragRectTransform.DOScale(1.5f, 0.4f);
                _dragRectTransform.DOMove(_dragSourceCell.transform.position, 0.3f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        _dragIcon.gameObject.SetActive(false);
                        _dragSourceCell.ConfigureSeedInCell();
                        _dragRectTransform.position = _dragSourceCell.transform.position;
                    });
            }
        }

        private void SetDragPosition(PointerEventData eventData)
        {
            if (_dragRectTransform == null || _canvas == null) return;

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