using System;
using UniRx;

namespace Scripts
{
    public interface IGameEvents
    {
        IObservable<float> OnPlayerHit { get; }
        void PlayerHit(float damage);
        
        IObservable<Unit> OnEnemyKilled { get; }
        void EnemyKilled();
    }
}