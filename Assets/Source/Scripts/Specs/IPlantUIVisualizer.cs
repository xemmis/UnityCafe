using Models.Plant;

namespace Specs
{
    public interface IPlantUIVisualizer
    {
        void VisualizeWish(PlantWishType plantWishType);
        void VisualizeReadyCondition();
        void ClearUI();
    }
}
