namespace Models.Furniture
{
    using Core;
    using Models.Plant;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class SeedsBuyManager : MonoBehaviour
    {
        [SerializeField] private List<PlantBehavior> _plants = new();
        public static SeedsBuyManager Instance { get; private set; } = null;

        private void Awake()
        {
            if (SeedsBuyManager.Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetPlant(PlantSO plant)
        {
            foreach (PlantBehavior plantBehavior in _plants)
            {
                if (plantBehavior.PlantData == null)
                {
                    plantBehavior.Initialize(plant);
                    return;
                }
            }
        }
    }
}
