using Core;
using Core.Food;
using UnityEngine;
using UnityEngine.UI;

namespace Models.Food
{
    public sealed class IngredientShopCell : MonoBehaviour
    {
        [SerializeField] private IngredientItem _ingredientData = null;
        [SerializeField] private Button _buyBtn = null;
        [SerializeField] private Image _ingredientSprite = null;
        [SerializeField] private int _purchaseAmount = 1;

        private void Awake()
        {
            _buyBtn.onClick.AddListener(HandleBuy);
        }

        private void Start()
        {
            if (_ingredientData == null) return;

            _ingredientSprite.sprite = _ingredientData.Icon;
        }

        private void OnDestroy()
        {
            _buyBtn.onClick.RemoveListener(HandleBuy);
        }

        private void HandleBuy()
        {
            if (_ingredientData == null) return;

            if (Wallet.TrySpendMoney(_ingredientData.Cost))
                BakeryInventory.Add(_ingredientData, _purchaseAmount);
        }
    }
}