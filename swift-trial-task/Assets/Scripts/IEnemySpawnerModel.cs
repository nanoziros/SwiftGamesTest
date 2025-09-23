using System;
using UniRx;

public interface IEnemySpawnerModel
{
    int MaxActiveEnemies { get; }
    int EnemySpawnBatch { get; }
    float SpawnInterval { get; }
    IObservable<Unit> OnSpawnEnemy { get; }
    void StartSpawning();
    void StopSpawning();
}