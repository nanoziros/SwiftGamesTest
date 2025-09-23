using UniRx;

namespace Scripts
{
    public interface IPlayerHudModel
    {
        IReadOnlyReactiveProperty<int> CurrentEnemiesDefeated { get; }
        public void SetEnemyKilled();
    }
}