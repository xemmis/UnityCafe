using UnityEngine;
namespace Utils
{
    public static class GameObjectExtensions
    {
        // TryGetComponentInChildren
        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T result) where T : Component
        {
            result = gameObject.GetComponentInChildren<T>();
            return result != null;
        }

        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T result, bool includeInactive) where T : Component
        {
            result = gameObject.GetComponentInChildren<T>(includeInactive);
            return result != null;
        }

        // TryGetComponentInParent
        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T result) where T : Component
        {
            result = gameObject.GetComponentInParent<T>();
            return result != null;
        }

        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T result, bool includeInactive) where T : Component
        {
            result = gameObject.GetComponentInParent<T>(includeInactive);
            return result != null;
        }
    }



    public static class ColorExtensions
    {
        public static Color Transparent = new Color(1f,1f, 1f, 0f);
        public static Color Visible = new Color(1f, 1f, 1f, 1f);
    }
}
