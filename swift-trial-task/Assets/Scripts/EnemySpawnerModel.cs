using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Scripts;
using UniRx;

public class EnemySpawnerModel : IEnemySpawnerModel
{
    public int MaxActiveEnemies => 30;
    public int EnemySpawnBatch => 3;
    public float SpawnInterval => 2f;

    private readonly Subject<Unit> _onSpawnEnemy = new();
    public IObservable<Unit> OnSpawnEnemy => _onSpawnEnemy;

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
        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < EnemySpawnBatch; i++)
            {
                _onSpawnEnemy.OnNext(Unit.Default);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(SpawnInterval), cancellationToken: token);
        }
    }
}