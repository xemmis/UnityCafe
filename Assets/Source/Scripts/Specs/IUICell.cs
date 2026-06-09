namespace Specs
{
    public interface IUICell<T>
    {
        bool IsEmpty { get; set; }
        void SetItem(T item);
        void ClearCell();
        T GetItem();
    }
}