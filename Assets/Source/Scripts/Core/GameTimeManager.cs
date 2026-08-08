namespace Core
{
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    public class GameTimeManager : MonoBehaviour
    {
        [SerializeField] private float _dayTimer = 5;
        private Coroutine _dayTimerRoutine = null;
        public static GameTimeManager Instance { get; private set; } = null;
        public UnityEvent<bool> OnDayConditionChange = null;

        private void Awake()
        {
            InitializeSingleton();

        }

        private void Start()
        {
            StartDay();

        }

        private void InitializeSingleton()
        {
            if (GameTimeManager.Instance == null)
            {
                GameTimeManager.Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartDay()
        {
            Wallet.ClearDayEarn();
            OnDayConditionChange?.Invoke(true);
            print("Start");
            _dayTimerRoutine = StartCoroutine(DayTimer());
        }

        public void EndDay()
        {
            OnDayConditionChange?.Invoke(false);
            StopCoroutine(_dayTimerRoutine);
            _dayTimerRoutine = null;
        }

        private IEnumerator DayTimer()
        {
            OnDayConditionChange?.Invoke(true);
            yield return new WaitForSeconds(_dayTimer);
            EndDay();
        }

        private void OnDestroy()
        {
            OnDayConditionChange = null;
        }
    }

    public sealed class GameEndCalculator
    {
        public GameEndCalculator(TextMeshProUGUI taxTmp, TextMeshProUGUI toPayTmp)
        {
            _taxTmp = taxTmp;
            _toPayTmp = toPayTmp;
        }

        private TextMeshProUGUI _taxTmp = null;
        private TextMeshProUGUI _toPayTmp = null;
        private int _taxAmount = 10;
        private int _payAmount;
        private float _taxMuiltiplyer = 1.35f;

        public void Calculate(int DayEarn)
        {
            _taxAmount = ((int)(_taxAmount * _taxMuiltiplyer));
            _payAmount = DayEarn * _taxAmount;
            _taxTmp.text = _taxAmount.ToString();
            _toPayTmp.text = DayEarn.ToString();
        }

        public bool ConfirmPay()
        {
            if (!Wallet.TrySpendMoney(_payAmount))
            {
                return false;
            }

            return true;
        }
    }
}