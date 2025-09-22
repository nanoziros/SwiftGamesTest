using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Slider _healthBar;
        
        [SerializeField, Range(float.Epsilon, 5f)]
        private float _speed;
        private Vector2 _velocity;

        public Vector2 Position => transform.position;
        public Vector2 Velocity => _velocity;

        public void Move(Vector2 direction)
        {
            var oldPosition = transform.position;
            _velocity = direction * _speed;
            transform.position = Vector3.Lerp(oldPosition, oldPosition + (Vector3)_velocity,
                Time.deltaTime);
        }
        
        public void OnMoveEnd()
        {
            _velocity = Vector2.zero;
        }

        public void UpdateHealth(float currentNormalizedHealth)
        {
            _healthBar.DOValue(currentNormalizedHealth, .2f).SetEase(Ease.OutSine);
        }
    }
}