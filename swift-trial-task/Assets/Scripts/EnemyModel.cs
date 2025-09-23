using System;
using UniRx;
using Zenject;

namespace Scripts
{
    public class EnemyModel : IEnemyModel
    {
        private const float MAX_HEALTH = 3f;
        private readonly ReactiveProperty<float> _currentHealth = new(MAX_HEALTH);
        
        public IReadOnlyReactiveProperty<float> CurrentHealth => _currentHealth;
        public float MaxHealth => MAX_HEALTH;
        public float VisibilityCheckInterval => 0.5f;
        public float EnemyDespawnDelay => 2;
        public float EnemyDamage => 1f;
        
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

        public void Reset()
        {
            _currentHealth.Value = MAX_HEALTH;
        }
        
        public class Factory : PlaceholderFactory<EnemyModel> { }
    }
}