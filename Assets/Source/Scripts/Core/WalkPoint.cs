using Models.Npc;
using Specs;
using UnityEngine;

namespace Core
{
    public sealed class WalkPoint : MonoBehaviour, IReservable
    {
        [SerializeField] private bool _isReserved = false;
        [SerializeField] private WalkType _type;
        public bool IsReserved => _isReserved;


        WalkType IReservable.Type => _type;

        private void Start()
        {
            WalkManager.Instance?.RegisterPoint(this, _type);
        }

        public void Reserve()
        {
            _isReserved = true;
        }

        public void CancelReservation()
        {
            _isReserved = false;
        }
    }
}