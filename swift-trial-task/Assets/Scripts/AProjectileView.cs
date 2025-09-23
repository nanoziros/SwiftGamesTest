using System;
using Scripts.Utils;
using UniRx;
using UnityEngine;

namespace Scripts
{
    public abstract class AProjectileView : MonoBehaviour
    {
        [SerializeField, Range(float.Epsilon, 20f)]
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
            _velocity = transform.Move(direction, _speed);
        }

        private void OnDisable()
        {
            _onDisabled.OnNext(Unit.Default);
        }
    }
}