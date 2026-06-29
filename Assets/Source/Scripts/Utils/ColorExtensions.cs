using UnityEngine;
namespace Utils
{
    public static class ColorExtensions
    {

        public static readonly Color Transparent = new Color(1, 1, 1, 0);
        public static Color Visible(float alpha = 1)
        {
            return new Color(1, 1, 1, alpha);
        }
    }
}
