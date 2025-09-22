using System;
using UniRx;
using UnityEngine;

namespace Scripts
{
    public abstract class AProjectileView : MonoBehaviour
    {
        [SerializeField, Range(float.Epsilon, 5f)]
        private float _speed;
        private Vector2 _velocity;
        public Vector2 Velocity => _velocity;
        public Vector2 Position => transform.position;

        private readonly Subject<Unit> _onDisabled = new();
        public IObservable<Unit> OnDisabled => _onDisabled;
        
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
        
        public void Move(Vector2 direction)
        {
            var oldPosition = transform.position;
            _velocity = direction * _speed;
            transform.position = Vector3.Lerp(oldPosition, oldPosition + (Vector3)_velocity,
                Time.deltaTime);
        }

        private void OnDisable()
        {
            _onDisabled.OnNext(Unit.Default);
        }
    }
}