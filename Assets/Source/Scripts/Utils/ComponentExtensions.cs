using UnityEngine;
namespace Utils
{
    public static class ComponentExtensions
    {
        public static bool TryGetComponentInChildren<T>(this Component component, out T result) where T : Component
        {
            result = component.GetComponentInChildren<T>();
            return result != null;
        }

        public static bool TryGetComponentInChildren<T>(this Component component, out T result, bool includeInactive) where T : Component
        {
            result = component.GetComponentInChildren<T>(includeInactive);
            return result != null;
        }

        public static bool TryGetComponentInParent<T>(this Component component, out T result) where T : Component
        {
            result = component.GetComponentInParent<T>();
            return result != null;
        }

        public static bool TryGetComponentInParent<T>(this Component component, out T result, bool includeInactive) where T : Component
        {
            result = component.GetComponentInParent<T>(includeInactive);
            return result != null;
        }
    }
}
