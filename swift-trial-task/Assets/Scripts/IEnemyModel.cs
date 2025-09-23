using UniRx;

namespace Scripts
{
    public interface IEnemyModel
    {
        public float MaxHealth { get; }
        IReadOnlyReactiveProperty<float> CurrentHealth { get; }
        public float VisibilityCheckInterval { get; }
        public float EnemyDespawnDelay { get; }
        public float EnemyDamage { get; }
        
        void TakeDamage(float damage);
        void Reset();
    }
}