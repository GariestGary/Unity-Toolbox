using UnityEngine;

namespace VolumeBox.Toolbox
{
    public static class ScreenUtils
    {
        private static Vector2 _screenRatio
        {
            get 
            {
                Vector2 ratio;
                ratio.x = (float)Screen.width / (float)Screen.currentResolution.width;
                ratio.y = (float)Screen.height / (float)Screen.currentResolution.height;
                return ratio;
            }
        }

        /// <summary>
        /// Calculates the horizontal layout position for a given index in a panel with a given number of elements.
        /// </summary>
        public static Vector2 GetHorizontalLayoutPosition(RectTransform panel, int count, int index)
        {
            index = Mathf.Clamp(index, 0, count - 1);
            float size = panel.rect.width / count;
            return new Vector2(panel.rect.min.x + size * 0.5f + size * index, panel.anchoredPosition.y);
        }

        /// <summary>
        /// Reorganizes the given RectTransform array horizontally according to the given panel.
        /// </summary>
        public static void ReorganizeRectsHorizontally(RectTransform[] rects, RectTransform panel)
        {
            for (int i = 0; i < rects.Length; i++)
            {
                rects[i].anchoredPosition = GetHorizontalLayoutPosition(panel, rects.Length, i);
            }
        }

        /// <summary>
        /// Converts delta from screen space to canvas space
        /// </summary>
        public static Vector2 PointerDeltaToCanvas(Vector2 point)
        {
            return point / _screenRatio;
        }

        /// <summary>
        /// Returns the screen-space position of a RectTransform in world space.
        /// </summary>
        public static Vector2 GetScreenRelativePosition(RectTransform rect)
        {
            return Camera.main.WorldToScreenPoint(rect.position);
        }

        /// <summary>
        /// Converts point from screen space to canvas space
        /// </summary>
        public static Vector2 PointerPositionToCanvas(Vector2 point)
        {
            point.x -= Screen.width * 0.5f;
            point.y -= Screen.height * 0.5f;
            point /= _screenRatio;

            return point;
        }

        /// <summary>
        /// Converts point from canvas space to world space
        /// </summary>
        public static Vector3 CanvasToWorldPosition(Vector2 canvasPos)
        {
            canvasPos.x += Screen.width * 0.5f;
            canvasPos.y += Screen.height * 0.5f;
            return Camera.main.ScreenToWorldPoint(canvasPos);
        }
    }
}
