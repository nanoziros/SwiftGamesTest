using System;
using UniRx;

namespace Scripts
{
    public class GameEvents : IDisposable, IGameEvents
    {
        private readonly Subject<float> _onPlayerHit = new();
        public IObservable<float> OnPlayerHit => _onPlayerHit;
        
        private readonly Subject<Unit> _onEnemyKilled = new();
        public IObservable<Unit> OnEnemyKilled => _onEnemyKilled;
        
        public void PlayerHit(float damage)
        {
            _onPlayerHit.OnNext(damage);
        }
        
        public void EnemyKilled()
        {
            _onEnemyKilled.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _onEnemyKilled?.Dispose();
            _onPlayerHit?.Dispose();
        }
    }
}