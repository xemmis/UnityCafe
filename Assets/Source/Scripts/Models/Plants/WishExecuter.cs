using Core;
using DG.Tweening;
using Specs;
using UnityEngine;

namespace Models.Plant
{
    public sealed class WishExecuter : MonoBehaviour
    {
        [SerializeField] private WishType _currentWishType = WishType.None;

        private Vector3 _startPosition;

        public bool CanDrag => _currentWishType != WishType.None;

        private IWish _wish => WishFactory.Instance?.CreateWish(_currentWishType);

        private void Awake()
        {
            _startPosition = transform.position;
        }

        public void OnDragStart()
        {

        }

        public void OnDragEnd()
        {
            // Здесь можно убрать визуал
        }

        public void MoveTo(Vector3 position)
        {
            DOTween.Kill(gameObject);
            transform.DOMove(position, 0.01f).SetEase(Ease.Linear).SetId(gameObject);
        }

        public void Execute(PlantBehavior plant)
        {
            _wish?.Execute(plant);
        }

        public void ReturnToStart()
        {
            DOTween.Kill(gameObject);
            transform.DOMove(_startPosition, 0.3f).SetEase(Ease.InBack).SetId(gameObject);
        }
    }
}


