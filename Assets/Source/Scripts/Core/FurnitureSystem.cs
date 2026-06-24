using UnityEngine;
using System.Collections.Generic;
using Models.Furniture;


namespace Core
{
    public sealed class FurnitureSystem : MonoBehaviour
    {
        private List<Furniture> _furnitures = new();

        public void HandleBuildMode()
        {
            GameCondition.ChangeBuildingModeCondition();
        }
    }
}