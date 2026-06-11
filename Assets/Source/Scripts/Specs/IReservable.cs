using Models;
using Models.Npc;

namespace Specs
{
    public interface IReservable
    {
        void Reserve();
        void CancelReservation();
        bool IsReserved { get; }
        WalkType Type { get; }
    }
}
