using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Scripts;
using UniRx;

public class CrossbowSpawnerModel : IProjectileSpawnerModel
{
    private readonly Subject<Unit> _onSpawnProjectile = new();
    public IObservable<Unit> OnSpawnProjectile => _onSpawnProjectile;

    public int MaxProjectiles => 5;
    public float InitialSpawnDelay => 1;
    public float SpawnInterval => 5;

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