using UniRx;

namespace Scripts
{
    public class PlayerHudModel : IPlayerHudModel
    {
        private readonly ReactiveProperty<int> _currentEnemiesDefeated = new(0);
        public IReadOnlyReactiveProperty<int> CurrentEnemiesDefeated => _currentEnemiesDefeated;
        
        public void SetEnemyKilled()
        {
            _currentEnemiesDefeated.Value ++;
        }
    }
}