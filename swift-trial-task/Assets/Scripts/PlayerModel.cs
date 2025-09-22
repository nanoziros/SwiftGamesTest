using UniRx;

namespace Scripts
{
    public class PlayerModel : IPlayerModel
    {
        private const float MAX_HEALTH = 10f;
        private readonly ReactiveProperty<float> _currentHealth = new(MAX_HEALTH);
        public IReadOnlyReactiveProperty<float> CurrentHealth => _currentHealth;
        public float MaxHealth => MAX_HEALTH;
        
        public void TakeDamage(float damage)
        {
            _currentHealth.Value -= damage;
        }
    }
}