namespace Core
{
    using Models.Plant;
    using System;
    using UnityEngine;

    [System.Serializable]
    public struct PlantData : IEquatable<PlantData>
    {
        [Header("Prefab Settings")]
        [SerializeField] private Plant _prefab;
        [SerializeField] private string _plantName;

        [Header("Default Configuration")]
        [SerializeField] private PlantSO _defaultPlantSO;

        [Header("Visual Override")]
        [SerializeField] private Material _overrideMaterial;
        [SerializeField] private Vector3 _customScale;

        public Plant Prefab => _prefab;
        public string PlantName => _plantName ?? _prefab?.name ?? "Unknown";
        public PlantSO DefaultPlantSO => _defaultPlantSO;
        public Material OverrideMaterial => _overrideMaterial;
        public Vector3 CustomScale => _customScale != Vector3.zero ? _customScale : Vector3.one;

        public bool IsValid => _prefab != null;
        public bool HasVisualOverride => _overrideMaterial != null || CustomScale != Vector3.one;

        public bool Equals(PlantData other)
        {
            return Equals(_prefab, other._prefab) &&
                   string.Equals(PlantName, other.PlantName);
        }

        public override bool Equals(object obj)
        {
            return obj is PlantData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_prefab != null ? _prefab.GetHashCode() : 0) * 397) ^
                       (PlantName != null ? PlantName.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"PlantData: {PlantName} (Prefab: {_prefab?.name})";
        }

        public static bool operator ==(PlantData left, PlantData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlantData left, PlantData right)
        {
            return !left.Equals(right);
        }
    }
}
