using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Scripts
{
    public class CrossbowSpawnerModel : IProjectileSpawnerModel
    {
        private readonly Subject<Unit> _onSpawnProjectile = new();

        public int MaxProjectiles => 5;
        private float InitialSpawnDelay => 1;
        private float SpawnInterval => 5;
        public IObservable<Unit> OnSpawnProjectile => _onSpawnProjectile;
        private CancellationTokenSource _cts;

        public void StartSpawning()
        {
            _cts = new CancellationTokenSource();
            SpawnLoop(_cts.Token).Forget();
        }

        public void StopSpawning()
        {
            _cts?.Cancel();
        }

        private async UniTaskVoid SpawnLoop(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(InitialSpawnDelay), cancellationToken: token);
            while (!token.IsCancellationRequested)
            {
                _onSpawnProjectile.OnNext(Unit.Default);
                await UniTask.Delay(TimeSpan.FromSeconds(SpawnInterval), cancellationToken: token);
            }
        }
    }
}