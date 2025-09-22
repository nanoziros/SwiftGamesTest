using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private RectTransform _healthBarRectTransform;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Slider _healthBar;
        
        [SerializeField, Range(float.Epsilon, 5f)]
        private float _speed;
        private Vector2 _velocity;
        
        private readonly Subject<Unit> _onEnabled = new();
        private readonly Subject<Unit> _onDisabled = new();
        private readonly Subject<Collider2D> _onTriggerEntered = new();

        public IObservable<Unit> OnEnabled => _onEnabled;
        public IObservable<Unit> OnDisabled => _onDisabled;
        public IObservable<Collider2D> OnTriggerEntered => _onTriggerEntered;

        public Vector2 Position => transform.position;
        public Bounds Bounds => GetFullBounds();
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        private void OnEnable()
        {
            _onEnabled.OnNext(Unit.Default);
        }

        private void OnDisable()
        {
            _onDisabled.OnNext(Unit.Default);
        }

        private Bounds GetFullBounds()
        {
            var bounds = _spriteRenderer.bounds;

            Vector3[] corners = new Vector3[4];
            _healthBarRectTransform.GetWorldCorners(corners);
            Bounds healthBounds = new(corners[0], Vector3.zero);
            
            for (int i = 1; i < 4; i++)
            {
                healthBounds.Encapsulate(corners[i]);
            }

            bounds.Encapsulate(healthBounds);

            return bounds;
        }
        
        public void Move(Vector2 direction)
        {
            var oldPosition = transform.position;
            _velocity = direction * _speed;
            transform.position = Vector3.Lerp(oldPosition, oldPosition + (Vector3)_velocity,
                Time.deltaTime);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            _onTriggerEntered.OnNext(other);
        }
    }
}