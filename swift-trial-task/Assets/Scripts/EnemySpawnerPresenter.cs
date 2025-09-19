using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Scripts
{
    // If later we want to have different kinds of spawners, we could replace the EnemyView with a generic Type and define several EnemySpawnerPresenter with different types during the dependency binding step
    public class EnemySpawnerPresenter : IInitializable
    {
        private readonly IEnemySpawnerModel _enemySpawnerModel;
        private readonly ICameraView _playerCamera;
        private readonly Transform _enemiesParent;
        private readonly EnemyView _enemyPrefab;

        private GameObjectPool<EnemyView> _enemyPool;
        
        public EnemySpawnerPresenter(EnemyView enemyPrefab, Transform enemiesParent, IEnemySpawnerModel enemySpawnerModel, ICameraView playerCamera)
        {
            _enemySpawnerModel = enemySpawnerModel;
            _playerCamera = playerCamera;
            _enemyPrefab = enemyPrefab;
            _enemiesParent = enemiesParent;
        }

        public void Initialize()
        {
            _enemyPool = new GameObjectPool<EnemyView>(_enemyPrefab, _enemySpawnerModel.MaxActiveEnemies, _enemiesParent);   
            StartSpawning().Forget();
        }
        
        private async UniTaskVoid StartSpawning()
        {
            var maxActiveEnemies = _enemySpawnerModel.MaxActiveEnemies;
            var spawnInterval = _enemySpawnerModel.SpawnInterval;
            
            // If the spawning loop can be cancelled, we could add a CancellationTokenSource check in here
            while (_enemyPool.Count < maxActiveEnemies)
            {
                // todo: spawn enemy here
                
                await UniTask.Delay(System.TimeSpan.FromSeconds(spawnInterval));
            }
        }
    }
}