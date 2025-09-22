using UniRx;

namespace Scripts
{
    public interface IEnemyModel
    {
        public float MaxHealth { get; }
        IReadOnlyReactiveProperty<float> CurrentHealth { get; }
        public float VisibilityCheckInterval { get; }
        public float EnemyDespawnDelay { get; }
        
        void TakeDamage(float damage);
        void Reset();
    }
}