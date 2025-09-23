using System;
using System.Collections.Generic;
using Scripts;
using Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;

public class EnemySpawnerPresenter : IInitializable, IEnemyProvider, IDisposable
{
    private readonly IEnemySpawnerModel _enemySpawnerModel;
    private readonly EnemyView _enemyPrefab;
    private readonly EnemyModel.Factory _enemyModelFactory;
    private readonly PlayerView _playerView;
    private readonly Camera _playerCamera;
    private readonly Transform _enemiesParent;
    private readonly CompositeDisposable _disposer;
    private readonly IGameEvents _gameEvents;

    private GameObjectPool<EnemyView> _enemyPool;
    private readonly Dictionary<EnemyView, EnemyPresenter> _enemyPresenters = new();
    private readonly List<EnemyPresenter> _visibleBuffer = new();

    public EnemySpawnerPresenter(
        EnemyView enemyPrefab,
        EnemyModel.Factory enemyModelFactory,
        PlayerView playerView,
        Camera playerCamera,
        [Inject(Id = PoolTransformIds.EnemiesParentId)] Transform enemiesParent,
        IEnemySpawnerModel enemySpawnerModel,
        IGameEvents gameEvents,
        CompositeDisposable disposer)
    {
        _enemyPrefab = enemyPrefab;
        _enemyModelFactory = enemyModelFactory;
        _playerView = playerView;
        _playerCamera = playerCamera;
        _enemiesParent = enemiesParent;
        _enemySpawnerModel = enemySpawnerModel;
        _gameEvents = gameEvents;
        _disposer = disposer;
    }

    public void Initialize()
    {
        _enemyPool = new GameObjectPool<EnemyView>(_enemyPrefab, _enemySpawnerModel.MaxActiveEnemies, _enemiesParent);

        _gameEvents.OnPlayerDied.Subscribe(_ => _enemySpawnerModel.StopSpawning()).AddTo(_disposer);
        _enemySpawnerModel.OnSpawnEnemy
            .Subscribe(_ => SpawnEnemy())
            .AddTo(_disposer);

        _enemySpawnerModel.StartSpawning();
    }

    private void SpawnEnemy()
    {
        if (!_enemyPool.HasAvailableObjects)
        {
            return;
        }

        var view = _enemyPool.Get();

        if (!_enemyPresenters.TryGetValue(view, out var presenter))
        {
            var enemyModel = _enemyModelFactory.Create();
            presenter = new EnemyPresenter(view, _playerView, enemyModel, _playerCamera, _gameEvents, _disposer);

            presenter.OnDespawn
                     .Subscribe(e => _enemyPool.Return(e))
                     .AddTo(_disposer);

            _enemyPresenters[view] = presenter;
        }

        presenter.Initialize();
        presenter.SetRandomOffScreenPosition();
        presenter.StartChasingPlayer();
    }

    public IReadOnlyList<EnemyPresenter> GetVisibleEnemies()
    {
        _visibleBuffer.Clear();
        foreach (var kv in _enemyPresenters)
        {
            if (!kv.Value.IsOffScreen)
                _visibleBuffer.Add(kv.Value);
        }
        return _visibleBuffer;
    }

    public void Dispose()
    {
        _disposer.Dispose();
        _enemySpawnerModel.StopSpawning();
    }
}