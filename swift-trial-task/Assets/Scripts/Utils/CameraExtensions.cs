using UnityEngine;

namespace Scripts.Utils
{
    public static class CameraExtensions
    {
        private enum ViewSide
        {
            Top = 0,
            Bottom = 1,
            Left = 2,
            Right = 3
        }
        
        public static Vector3 GetRandomOffScreenPosition(this Camera camera, Bounds targetObjectBounds, Vector3 biasDirection)
        {
            var worldRect = camera.GetWorldRect();
            
            float halfWidth = targetObjectBounds.extents.x;
            float halfHeight = targetObjectBounds.extents.y;

            ViewSide side = SelectBiasedSide(biasDirection);
            float x = 0f, y = 0f;

            switch (side)
            {
                case ViewSide.Top:
                    y = worldRect.yMax + halfHeight;
                    x = Random.Range(worldRect.xMin + halfWidth, worldRect.xMax - halfWidth);
                    break;
                case ViewSide.Bottom:
                    y = worldRect.yMin - halfHeight;
                    x = Random.Range(worldRect.xMin + halfWidth, worldRect.xMax - halfWidth);
                    break;
                case ViewSide.Left:
                    x = worldRect.xMin - halfWidth;
                    y = Random.Range(worldRect.yMin + halfHeight, worldRect.yMax - halfHeight);
                    break;
                case ViewSide.Right:
                    x = worldRect.xMax + halfWidth;
                    y = Random.Range(worldRect.yMin + halfHeight, worldRect.yMax - halfHeight);
                    break;
            }

            return new Vector3(x, y, 0f);
        }
        
        
        // todo: check if we can use this in other bias calculations or we can encapsulate in a dedicated util
        private static ViewSide SelectBiasedSide(Vector2 biasDirection)
        {
            if (biasDirection == Vector2.zero)
            {
                return GetRandomViewSide();
            }

            Vector2 dir = biasDirection.normalized;

            float topWeight = Mathf.Max(0f, dir.y);
            float bottomWeight = Mathf.Max(0f, -dir.y);
            float rightWeight = Mathf.Max(0f, dir.x);
            float leftWeight = Mathf.Max(0f, -dir.x);

            float total = topWeight + bottomWeight + leftWeight + rightWeight;
            
            if (total <= 0f)
            {
                return GetRandomViewSide();
            }

            float random = Random.value * total;
            if (random < topWeight)
            {
                return ViewSide.Top;
            }
            random -= topWeight;
            if (random < bottomWeight)
            {
                return ViewSide.Bottom;
            }
            random -= bottomWeight;
            if (random < leftWeight)
            {
                return ViewSide.Left;
            }
            return ViewSide.Right;
        }

        private static ViewSide GetRandomViewSide()
        {
            return (ViewSide)Random.Range(0, 4);
        }
        
        public static bool IsOffScreen(this Camera camera, Bounds bounds)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Rect cameraRect =camera.GetWorldRect();
            return max.x <= cameraRect.xMin || min.x >= cameraRect.xMax ||
                   max.y <= cameraRect.yMin || min.y >= cameraRect.yMax;
        }


        private static Rect GetWorldRect(this Camera camera)
        {
            var bottomLeft = camera.ViewportToWorldPoint(Vector3.zero);
            var topRight = camera.ViewportToWorldPoint(Vector3.one);
            return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
        }
    }
}