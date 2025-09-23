using DG.Tweening;
using Scripts.Utils;
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
            _velocity = transform.Move(direction, _speed);
        }
        
        public void OnMoveEnd()
        {
            _velocity = Vector2.zero;
        }

        public void UpdateHealth(float currentNormalizedHealth)
        {
            _healthBar.DOValue(currentNormalizedHealth, .2f).SetEase(Ease.OutSine);
        }

        public void Disable()
        {
            // todo: here could play a death animation
        }
    }
}