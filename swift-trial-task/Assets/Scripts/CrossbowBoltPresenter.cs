using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts
{
    public class CrossbowBoltPresenter : IProjectilePresenter
    {
        private CrossbowBoltView _view;
        private readonly PlayerView _playerView;
        private readonly ICrossbowBoltModel _crossbowBoltModel;
        private readonly Camera _camera;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGameEvents _gameEvents;
        private readonly CompositeDisposable _disposer;
        private CancellationTokenSource _moveCts;

        private readonly Subject<CrossbowBoltView> _onDespawn = new();
        public IObservable<AProjectileView> OnDespawn => _onDespawn;

        public CrossbowBoltPresenter(
            PlayerView playerView,
            ICrossbowBoltModel model,
            Camera camera,
            IGameEvents gameEvents,
            IEnemyProvider enemySpawnerPresenter,
            CompositeDisposable disposer)
        {
            _enemyProvider = enemySpawnerPresenter;
            _playerView = playerView;
            _crossbowBoltModel = model;
            _camera = camera;
            _gameEvents = gameEvents;
            _disposer = disposer;
        }

        public void Initialize(AProjectileView view)
        {
            _view = view as CrossbowBoltView
                    ?? throw new InvalidOperationException($"Expected CrossbowBoltView, got {view.GetType()}");
            
            _gameEvents.OnPlayerDied.Subscribe(_ => OnViewDisabled()).AddTo(_disposer);
            view.OnDisabled.Subscribe(_ => OnViewDisabled()).AddTo(_disposer);
        }
        
        public void Fire()
        {
            _moveCts?.Cancel();
            _moveCts = new CancellationTokenSource();

            _view.SetPosition(_playerView.Position);

            FireProjectile(_moveCts.Token).Forget();
        }

        private async UniTaskVoid FireProjectile(CancellationToken token)
        {
            var randomDirection = Random.insideUnitCircle.normalized;
            Vector2 projectileDirection = GetProjectileDirection(randomDirection, _crossbowBoltModel.InitialBiasTowardsEnemy);

            await MoveInDirection(projectileDirection, token);
        }

        private Vector2 GetProjectileDirection(Vector2 defaultDirection, float biasTowardsEnemy)
        {
            var targetDirection = defaultDirection;
            var visibleEnemies = _enemyProvider.GetVisibleEnemies();
            var visibleEnemiesCount = visibleEnemies.Count;
            if (visibleEnemiesCount > 0)
            {
                var targetEnemy = visibleEnemies[Random.Range(0, visibleEnemiesCount)];
                var toEnemy = (targetEnemy.Position - _view.Position).normalized;
                
                targetDirection = Vector2.Lerp(targetDirection, toEnemy, biasTowardsEnemy);
            }

            return targetDirection;
        }

        private async UniTask MoveInDirection(Vector2 direction, CancellationToken token)
        {
            float distanceTravelled = 0f;
            float maxDistance = _crossbowBoltModel.MaxDistance;
            
            while (distanceTravelled < maxDistance && !token.IsCancellationRequested)
            {
                direction = TryBounceProjectile(direction);
                
                _view.Move(direction);
                distanceTravelled += _view.Velocity.magnitude * Time.deltaTime;
                
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            _onDespawn.OnNext(_view);
        }

        private Vector2 TryBounceProjectile(Vector2 direction)
        {
            Vector2 screenPos = _camera.WorldToViewportPoint(_view.Position);
            bool bouncedThisFrame = false;

            if (screenPos.x < 0f)
            {
                direction.x = Mathf.Abs(direction.x); 
                bouncedThisFrame = true;
            }
            else if (screenPos.x > 1f)
            {
                direction.x = -Mathf.Abs(direction.x); 
                bouncedThisFrame = true;
            }

            if (screenPos.y < 0f)
            {
                direction.y = Mathf.Abs(direction.y);
                bouncedThisFrame = true;
            }
            else if (screenPos.y > 1f)
            {
                direction.y = -Mathf.Abs(direction.y);
                bouncedThisFrame = true;
            }

            if (bouncedThisFrame)
            {
                direction = GetProjectileDirection(direction.normalized, _crossbowBoltModel.AfterBounceBiasTowardsEnemy);
            }

            return direction;
        }

        private void OnViewDisabled()
        {
            _moveCts?.Cancel();
            _moveCts?.Dispose();
            _moveCts = null;
        }
    }
}