using UnityEngine;

namespace Scripts.Utils
{
    public static class TransformExtensions
    {
        public static Vector2 Move(this Transform transform, Vector2 direction, float speed)
        {
            var oldPosition = transform.position;
            var velocity = direction * speed;
            transform.position = Vector3.Lerp(oldPosition, oldPosition + (Vector3)velocity,
                Time.deltaTime);
            return velocity;
        }
    }
}