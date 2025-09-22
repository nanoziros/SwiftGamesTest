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
        
        public void TakeDamage(float damage)
        {
            _currentHealth.Value -= damage;
        }

        public void Reset()
        {
            _currentHealth.Value = MAX_HEALTH;
        }
        
        public class Factory : PlaceholderFactory<EnemyModel> { }
    }
}