using UniRx;
using UnityEngine;
using Zenject;

namespace Scripts
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private JoystickView _joyStick;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private CameraView _cameraView;
        
        [SerializeField] private EnemyView _enemyPrefab;
        [SerializeField] private Transform _enemiesParent;

        private readonly CompositeDisposable _disposer = new();
        
        private void OnDestroy()
        {
            _disposer.Dispose();
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CompositeDisposable>().AsSingle();

            Container.BindInstance(_playerView);
            Container.BindInstance(_joyStick);
            
            Container.BindInterfacesTo<CameraView>().FromInstance(_cameraView).AsSingle();
            
            Container.BindInterfacesTo<PlayerModel>().AsSingle();
            Container.BindInterfacesTo<PlayerPresenter>().AsSingle().NonLazy();
            
            Container.BindInstance(_enemiesParent).AsSingle();
            Container.BindInstance(_enemyPrefab).AsSingle();     
            Container.BindInterfacesTo<EnemySpawnerModel>().AsSingle();
            Container.BindInterfacesTo<EnemySpawnerPresenter>().AsSingle().NonLazy();
        }
    }
}