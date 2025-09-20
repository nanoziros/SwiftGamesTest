using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace Scripts
{
    public class EnemySpawnerPresenter : IInitializable
    {
        private readonly IEnemySpawnerModel _enemySpawnerModel;
        private readonly Camera _playerCamera;
        private readonly Transform _enemiesParent;
        private readonly EnemyView _enemyPrefab;
        private readonly IEnemyModel _enemyModel;
        private readonly PlayerView _playerView;
        private readonly CompositeDisposable _disposer;

        private GameObjectPool<EnemyView> _enemyPool;
        private readonly Dictionary<EnemyView, EnemyPresenter> _enemyPresenters = new();
        private CancellationTokenSource _spawnLoopCancellationToken;

        public EnemySpawnerPresenter(EnemyView enemyPrefab, IEnemyModel enemyModel, PlayerView playerView, Transform enemiesParent, IEnemySpawnerModel enemySpawnerModel, Camera playerCamera, CompositeDisposable disposer)
        {
            _enemySpawnerModel = enemySpawnerModel;
            _playerCamera = playerCamera;
            _enemyPrefab = enemyPrefab;
            _playerView = playerView;
            _enemiesParent = enemiesParent;
            _enemyModel = enemyModel;
            _disposer = disposer;
        }

        public void Initialize()
        {
            _spawnLoopCancellationToken = new CancellationTokenSource();
            _enemyPool = new GameObjectPool<EnemyView>(_enemyPrefab, _enemySpawnerModel.MaxActiveEnemies, _enemiesParent);   
            SpawnEnemies(_spawnLoopCancellationToken.Token).Forget();
        }
        
        private async UniTaskVoid SpawnEnemies(CancellationToken token)
        {
            var spawnInterval = TimeSpan.FromSeconds(_enemySpawnerModel.SpawnInterval);
            var enemyBatch = _enemySpawnerModel.EnemySpawnBatch;
            while (!token.IsCancellationRequested)
            {
                SpawnEnemyBatch(enemyBatch);
                await UniTask.Delay(spawnInterval, cancellationToken: token);
            }
        }

        private void SpawnEnemyBatch(int enemyBatch)
        {
            for (int i = 0; i < enemyBatch; i++)
            {
                if (_enemyPool.HasAvailableObjects)
                {
                    SpawnEnemy();
                }
            }
        }

        private void SpawnEnemy()
        {
            var view = _enemyPool.Get();
            if (!_enemyPresenters.TryGetValue(view, out var presenter))
            {
                presenter = new EnemyPresenter(view, _playerView, _enemyModel, _playerCamera, _enemyPool, _disposer);
                _enemyPresenters[view] = presenter;
            }
            presenter.SetRandomOffScreenState();
        }
    }
}