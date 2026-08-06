namespace Core
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class GameEndManager : MonoBehaviour
    {

        [SerializeField] private Transform _popap = null;
        [SerializeField] private Transform _open = null;
        [SerializeField] private Transform _close = null;
        [SerializeField] private TextMeshProUGUI _taxTmp = null;
        [SerializeField] private TextMeshProUGUI _dayEarn = null;
        [SerializeField] private TextMeshProUGUI _toPayTmp = null;
        [SerializeField] private Button _confirmBtn = null;
        private GameEndCalculator _calculator = null;

        private void Start()
        {
            GameTimeManager.Instance.OnDayConditionChange.AddListener(HandleDayChange);
            _calculator = new GameEndCalculator(_taxTmp, _toPayTmp);
        }

        private void OnDestroy()
        {
            GameTimeManager.Instance?.OnDayConditionChange?.RemoveListener(HandleDayChange);
        }

        public void HandleDayChange(bool condition)
        {
            if (condition) ClosePopap();
            else ShowPopap();
        }

        public void ClosePopap()
        {
            _popap.DOMove(_close.position, .5f);
            Wallet.ClearDayEarn();
        }

        public void ShowPopap()
        {
            _popap.DOMove(_open.position, .5f);
            _dayEarn.text = Wallet.DayEarnings.ToString();
            _calculator.Calculate(Wallet.DayEarnings);
        }

        public void TryConfirmPay()
        {
            if (_calculator.ConfirmPay())
            {
                GameTimeManager.Instance.StartDay();
            }
        }
    }
}