using System;
using UniRx;

namespace Scripts
{
    public interface IPlayerModel
    {
        void TakeDamage(float damage);
        public float MaxHealth { get; }
        IReadOnlyReactiveProperty<float> CurrentHealth { get; }
        public IObservable<Unit> OnDeath {get; }
    }
}