using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static Vector2 GetHorizontalLayoutPosition(RectTransform panel, int count, int index)
        {
            index = Mathf.Clamp(index, 0, count - 1);

            float size = panel.rect.width / count;

            return new Vector2(panel.rect.min.x + size * 0.5f + size * index, panel.anchoredPosition.y);

        }

        public static void ReorganizeRectsHorizontally(RectTransform[] rects, RectTransform panel)
        {
            for (int i = 0; i < rects.Length; i++)
            {
                rects[i].anchoredPosition = GetHorizontalLayoutPosition(panel, rects.Length, i);
            }
        }


        public static Vector2 PointerDeltaToCanvas(Vector2 point)
        {
            return point / _screenRatio;
        }

        public static Vector2 GetScreenRelativePosition(RectTransform rect)
        {
            return Camera.main.WorldToScreenPoint(rect.position);
        }

        public static Vector2 PointerPositionToCanvas(Vector2 point)
        {
            point.x -= Screen.width * 0.5f;
            point.y -= Screen.height * 0.5f;
            point /= _screenRatio;

            return point;
        }

        public static Vector3 CanvasToWorldPosition(Vector2 canvasPos)
        {
            canvasPos.x += Screen.width * 0.5f;
            canvasPos.y += Screen.height * 0.5f;
            return Camera.main.ScreenToWorldPoint(canvasPos);
        }
    }
}
