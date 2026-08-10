using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Models.Plant
{
    public sealed class SeedShopCell : MonoBehaviour
    {
        [SerializeField] private PlantSO _plantData = null;
        [SerializeField] private Button _buyBtn = null;
        [SerializeField] private Image _seedSprite = null;
        [SerializeField] private bool _singlePurchase = false;

        private bool _isSolded = false;

        private void Awake()
        {
            _buyBtn.onClick.AddListener(HandleBuy);
        }

        private void Start()
        {
            if (_plantData == null) return;

            if (_plantData.IsQuestReward)
                Debug.LogWarning($"[SeedShopCell] '{_plantData.name}' помечен как IsQuestReward, но выставлен на продажу в магазине!", this);

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
                SeedInventory.Add(_plantData);

                if (_singlePurchase)
                {
                    _buyBtn.onClick.RemoveListener(HandleBuy);
                    _isSolded = true;
                }
            }
        }
    }
}