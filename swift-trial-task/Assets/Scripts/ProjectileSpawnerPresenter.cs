using System;
using System.Collections.Generic;
using Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace Scripts
{
    public class ProjectileSpawnerPresenter : IInitializable, IDisposable
    {
        private readonly IProjectileSpawnerModel _model;
        private readonly Transform _projectilesParent;
        private readonly IFactory<IProjectilePresenter> _presenterFactory;
        private readonly AProjectileView _projectilePrefab;
        private readonly CompositeDisposable _disposer;

        private GameObjectPool<AProjectileView> _projectilePool;
        private readonly Dictionary<AProjectileView, IProjectilePresenter> _presenters = new();

        public ProjectileSpawnerPresenter(
            IProjectileSpawnerModel projectileSpawnerModel,
            AProjectileView projectilePrefab,
            [Inject(Id = PoolTransformIds.ProjectilesParentId)] Transform projectilesParent,
            IProjectilePresenter.Factory presenterFactory,
            CompositeDisposable disposer)
        {
            _model = projectileSpawnerModel;
            _projectilePrefab = projectilePrefab;
            _projectilesParent = projectilesParent;
            _presenterFactory = presenterFactory;
            _disposer = disposer;
        }

        public void Initialize()
        {
            _projectilePool = new GameObjectPool<AProjectileView>(_projectilePrefab, _model.MaxProjectiles, _projectilesParent);
            _model.OnSpawnProjectile
                  .Subscribe(_ => SpawnProjectile())
                  .AddTo(_disposer);

            _model.StartSpawning();
        }

        private void SpawnProjectile()
        {
            if (!_projectilePool.HasAvailableObjects)
            {
                return;
            }

            var view = _projectilePool.Get();
            if (!_presenters.TryGetValue(view, out var presenter))
            {
                presenter = _presenterFactory.Create();
                presenter.Initialize(view);

                presenter.OnDespawn
                         .Subscribe(p => _projectilePool.Return(p))
                         .AddTo(_disposer);

                _presenters[view] = presenter;
            }

            presenter.Fire();
        }

        public void Dispose()
        {
            _disposer.Dispose();
            _model.StopSpawning();
        }
    }
}