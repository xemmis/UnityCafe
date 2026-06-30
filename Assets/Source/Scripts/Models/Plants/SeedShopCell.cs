using Core;
using Models.Furniture;
using UnityEngine;
using UnityEngine.UI;


namespace Models.Plant
{
    public sealed class SeedShopCell : MonoBehaviour
    {
        [SerializeField] private PlantSO _plantData = null;
        [SerializeField] private Button _buyBtn = null;
        [SerializeField] private Image _seedSprite = null;
        [SerializeField] private bool _isSolded = false;

        private void Awake()
        {
            _buyBtn.onClick.AddListener(HandleBuy);
        }

        private void Start()
        {
            if (_plantData == null) return;

            _seedSprite.sprite = _plantData.Ingredinet.Icon;
        }

        private void OnDestroy()
        {
            _buyBtn.onClick.RemoveListener(HandleBuy);
        }

        private void HandleBuy()
        {
            if (_isSolded) return;

            if (Wallet.TrySpendMoney(_plantData.PlantCost))
            {
                SeedsBuyManager.Instance.SetPlant(_plantData);
                _buyBtn.onClick.RemoveListener(HandleBuy);
                _isSolded = true;
            }
        }
    }
}