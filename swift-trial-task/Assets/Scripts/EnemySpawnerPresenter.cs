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
    public class EnemySpawnerPresenter : IInitializable, IEnemyProvider
    {
        private readonly IEnemySpawnerModel _enemySpawnerModel;
        private readonly Camera _playerCamera;
        private readonly Transform _enemiesParent;
        private readonly EnemyView _enemyPrefab;
        private readonly EnemyModel.Factory _enemyModelFactory;
        private readonly PlayerView _playerView;
        private readonly CompositeDisposable _disposer;
        private readonly IPlayerHitReceiver _playerHitReceiver;

        private GameObjectPool<EnemyView> _enemyPool;
        private readonly Dictionary<EnemyView, EnemyPresenter> _enemyPresenters = new();
        private CancellationTokenSource _spawnLoopCts;
        
        private readonly List<EnemyPresenter> _visibleBuffer = new();
        
        public EnemySpawnerPresenter(
            EnemyView enemyPrefab, 
            EnemyModel.Factory enemyModelFactory, 
            PlayerView playerView,
            [Inject(Id = PoolTransformIds.EnemiesParentId)] Transform enemiesParent, 
            IEnemySpawnerModel enemySpawnerModel, 
            Camera playerCamera, 
            IPlayerHitReceiver playerHitReceiver,
            CompositeDisposable disposer)
        {
            _enemyModelFactory = enemyModelFactory;
            _enemySpawnerModel = enemySpawnerModel;
            _playerCamera = playerCamera;
            _enemyPrefab = enemyPrefab;
            _playerView = playerView;
            _enemiesParent = enemiesParent;
            _disposer = disposer;
            _playerHitReceiver = playerHitReceiver;
        }

        public void Initialize()
        {
            _spawnLoopCts = new CancellationTokenSource();
            _enemyPool = new GameObjectPool<EnemyView>(_enemyPrefab, _enemySpawnerModel.MaxActiveEnemies, _enemiesParent);   
            SpawnEnemies(_spawnLoopCts.Token).Forget();
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
                var enemyModel = _enemyModelFactory.Create();
                presenter = new EnemyPresenter(view, _playerView, enemyModel, _playerCamera, _playerHitReceiver, _disposer);
                presenter.OnDespawn.Subscribe(enemyView => _enemyPool.Return(enemyView)).AddTo(_disposer);
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
                {
                    _visibleBuffer.Add(kv.Value);
                }
            }
            return _visibleBuffer;
        }
    }
}