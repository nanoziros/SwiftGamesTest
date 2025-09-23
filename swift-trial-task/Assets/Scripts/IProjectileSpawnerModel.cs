using System;
using UniRx;

namespace Scripts
{
    public interface IProjectileSpawnerModel
    {
        int MaxProjectiles { get; }
        IObservable<Unit> OnSpawnProjectile { get; }
        void StartSpawning();
        void StopSpawning();
    }
}