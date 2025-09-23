using System;
using UniRx;

namespace Scripts
{
    public interface IEnemySpawnerModel
    {
        int MaxActiveEnemies { get; }
        IObservable<Unit> OnSpawnEnemy { get; }
        void StartSpawning();
        void StopSpawning();
    }
}