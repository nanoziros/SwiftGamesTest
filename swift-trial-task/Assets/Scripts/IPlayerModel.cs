using UniRx;

namespace Scripts
{
    public interface IPlayerModel
    {
        void TakeDamage(float damage);
        IReadOnlyReactiveProperty<float> CurrentHealth { get; }
        public float MaxHealth { get; }
    }
}