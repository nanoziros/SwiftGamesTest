using System;
using UniRx;

namespace Scripts
{
    public class PlayerModel : IPlayerModel
    {
        private const float MAX_HEALTH = 10f;
        public float MaxHealth => MAX_HEALTH;
        private readonly ReactiveProperty<float> _currentHealth = new(MAX_HEALTH);
        public IReadOnlyReactiveProperty<float> CurrentHealth => _currentHealth;
        private readonly Subject<Unit> _onDeath = new();
        public IObservable<Unit> OnDeath => _onDeath;

        public void TakeDamage(float damage)
        {
            _currentHealth.Value -= damage;
            if (_currentHealth.Value <= 0)
            {
                _onDeath.OnNext(Unit.Default);
            }
        }
    }
}