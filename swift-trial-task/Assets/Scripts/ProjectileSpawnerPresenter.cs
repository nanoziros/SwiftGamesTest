using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Scripts
{
    public class ProjectileSpawnerPresenter : IInitializable
    {
        private readonly IProjectileSpawnerModel _model;
        private readonly Transform _projectilesParent;
        private readonly AProjectileView _projectilePrefab;
        private readonly IFactory<IProjectilePresenter> _presenterFactory;

        private CancellationTokenSource _spawnLoopCancellationToken;
        private GameObjectPool<AProjectileView> _projectilePool;
        private readonly Dictionary<AProjectileView, IProjectilePresenter> _presenters = new();
        
        
        public ProjectileSpawnerPresenter(
            IProjectileSpawnerModel projectileSpawnerModel,
            AProjectileView projectilePrefab,
            [Inject(Id = PoolTransformIds.ProjectilesParentId)] Transform projectilesParent,
            IProjectilePresenter.Factory presenterFactory)
        {
            _model = projectileSpawnerModel;
            _projectilePrefab = projectilePrefab;
            _projectilesParent = projectilesParent;
            _presenterFactory = presenterFactory;
        }
        
        public void Initialize()
        {
            _projectilePool = new GameObjectPool<AProjectileView>(_projectilePrefab, _model.MaxProjectiles, _projectilesParent);
            
            _spawnLoopCancellationToken = new CancellationTokenSource();
            SpawnProjectiles(_spawnLoopCancellationToken.Token).Forget();
        }
        
        private async UniTaskVoid SpawnProjectiles(CancellationToken token)
        {
            var spawnInterval = TimeSpan.FromSeconds(_model.SpawnInterval);

            while (!token.IsCancellationRequested)
            {
                if (_projectilePool.HasAvailableObjects)
                {
                    var view = _projectilePool.Get();

                    if (!_presenters.TryGetValue(view, out var presenter))
                    {
                        presenter = _presenterFactory.Create();
                        _presenters[view] = presenter;
                    }

                    presenter.Fire();
                }

                await UniTask.Delay(spawnInterval, cancellationToken: token);
            }
        }
    }
}