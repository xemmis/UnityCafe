using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BuyableWalkPoint : WalkPoint
    {
        [SerializeField] private SpriteRenderer _renderer = null;

        private void Awake()
        {
            if (_renderer == null)
                _renderer = GetComponent<SpriteRenderer>();
        }

        public void ChangeColor(Color color)
        {
            _renderer.color = color;
        }
    }
}