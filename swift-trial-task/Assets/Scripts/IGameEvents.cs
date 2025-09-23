using System;
using UniRx;

namespace Scripts
{
    public interface IGameEvents
    {
        IObservable<Unit> OnPlayerDied { get; }
        void PlayerDied();
        
        IObservable<float> OnPlayerHit { get; }
        void PlayerHit(float damage);
        
        IObservable<Unit> OnEnemyKilled { get; }
        void EnemyKilled();
    }
}