using Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace Scripts
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private JoystickView _joyStick;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private Camera _camera;
        
        [SerializeField] private AProjectileView _projectilePrefab;
        [SerializeField] private Transform _projectilesParent;
        
        [SerializeField] private EnemyView _enemyPrefab;
        [SerializeField] private Transform _enemiesParent;

        private readonly CompositeDisposable _disposer = new();
        
        private void OnDestroy()
        {
            _disposer.Dispose();
        }

        public override void InstallBindings()
        {
            BindCore();
            BindProjectiles();
            BindPlayer();
            BindEnemies();
        }

        private void BindCore()
        {
            Container.BindInterfacesAndSelfTo<CompositeDisposable>().AsSingle();
            Container.BindInstance(_playerView);
            Container.BindInstance(_joyStick);
            Container.BindInstance(_camera);
        }

        private void BindEnemies()
        {
            Container.BindInterfacesTo<EnemyModel>().AsSingle();
            Container.BindInstance(_enemiesParent).WithId(PoolTransformIds.EnemiesParentId);
            Container.BindInstance(_enemyPrefab).AsSingle();     
            Container.BindInterfacesTo<EnemySpawnerModel>().AsSingle();
            Container.BindInterfacesTo<EnemySpawnerPresenter>().AsSingle().NonLazy();
        }

        private void BindPlayer()
        {
            Container.BindInterfacesTo<PlayerModel>().AsSingle();
            Container.BindInterfacesTo<PlayerPresenter>().AsSingle().NonLazy();
        }

        private void BindProjectiles()
        {
            Container.BindInterfacesTo<ProjectileSpawnerPresenter>().AsSingle().NonLazy();
            Container.BindInstance(_projectilesParent).WithId(PoolTransformIds.ProjectilesParentId);
            Container.Bind<AProjectileView>()
                .FromInstance(_projectilePrefab)
                .AsSingle();
            BindCrosbowBolt();
        }

        private void BindCrosbowBolt()
        {
            Container.BindFactory<IProjectilePresenter, IProjectilePresenter.Factory>()
                .To<CrossbowBoltPresenter>() 
                .AsTransient();
            Container.BindInterfacesTo<CrossbowSpawnerModel>().AsSingle();
        }
    }
}