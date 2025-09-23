using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Scripts.Utils;
using UniRx;
using UnityEngine;

namespace Scripts
{
    public class EnemyPresenter
    {
        private readonly IGameEvents _gameEvents;
        private readonly PlayerView _playerView;
        private readonly EnemyView _enemyView;
        private readonly IEnemyModel _model;
        private readonly Camera _camera;
        private bool _isOffScreen = true;
        private CancellationTokenSource _visibilityCts;
        private CancellationTokenSource _despawnCts;
        private CancellationTokenSource _chasePlayerCts;

        private readonly Subject<EnemyView> _onDespawn = new();
        public IObservable<EnemyView> OnDespawn => _onDespawn;
        public bool IsOffScreen => _isOffScreen;
        public Vector2 Position => _enemyView.Position;

        public EnemyPresenter(
            EnemyView enemyEnemyView,
            PlayerView playerView,
            IEnemyModel model,
            Camera camera,
            IGameEvents gameEvents,
            CompositeDisposable disposer)
        {
            _playerView = playerView;
            _enemyView = enemyEnemyView;
            _model = model;
            _camera = camera;
            _gameEvents = gameEvents;
            SubscribeEvents(disposer);
            StartVisibilityCheckLoop();
        }

        private void SubscribeEvents(CompositeDisposable disposer)
        {
            _gameEvents.OnPlayerDied.Subscribe(_ => StopEnemyLoops()).AddTo(disposer);
            _model.OnDeath.Subscribe(_=> KillEnemy()).AddTo(disposer);
            _model.CurrentHealth
                .Subscribe(currentHealth =>
                {
                    _enemyView.UpdateHealth(currentHealth / _model.MaxHealth);
                })
                .AddTo(disposer);
            _enemyView.OnGameObjectEntered.Subscribe(OnGameObjectCollision).AddTo(disposer);
            
            _enemyView.OnEnabled.Subscribe(_=>
            {
                StartChasingPlayer();
                StartVisibilityCheckLoop();
            }).AddTo(disposer);
            _enemyView.OnDisabled.Subscribe(_ =>
            {
                StopEnemyLoops();
            }).AddTo(disposer);
        }

        private void KillEnemy()
        {
            StopEnemyLoops();
            _gameEvents.EnemyKilled();
            _onDespawn.OnNext(_enemyView);
        }

        private void StopEnemyLoops()
        {
            StopChasingPlayer();
            StopVisibilityCheckLoop();
            StopDespawnDelay();
        }

        public void Initialize()
        {
            _model.Reset();
        }

        public void SetRandomOffScreenPosition()
        {
            var targetPosition = _camera.GetRandomOffScreenPosition(_enemyView.Bounds, _playerView.Velocity);
            
            // todo: also guarantee the enemy is within the map bounds
            _enemyView.SetPosition(targetPosition);
        }
        
        public void StartChasingPlayer()
        {
            if (_chasePlayerCts is { IsCancellationRequested: false })
            {
                return;
            }

            _chasePlayerCts?.Dispose();
            _chasePlayerCts = new CancellationTokenSource();
            ChasePlayer(_chasePlayerCts.Token).Forget();
        }

        private void StopChasingPlayer()
        {
            _chasePlayerCts?.Cancel();
            _chasePlayerCts?.Dispose();
            _chasePlayerCts = null;
        }

        private async UniTaskVoid ChasePlayer(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _playerView)
            {
                var directionToPlayer = (_playerView.Position - _enemyView.Position).normalized;
                _enemyView.Move(directionToPlayer);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
            }
        }
        
        private void StartVisibilityCheckLoop()
        {
            if (_visibilityCts is { IsCancellationRequested: false })
            {
                return;
            }

            _visibilityCts?.Dispose();
            _visibilityCts = new CancellationTokenSource();
            CheckEnemyVisibility(_visibilityCts.Token).Forget();
        }
        
        private async UniTaskVoid CheckEnemyVisibility(CancellationToken token)
        {
            var interval = TimeSpan.FromSeconds(_model.VisibilityCheckInterval);

            while (!token.IsCancellationRequested)
            {
                CheckVisibility();
                await UniTask.Delay(interval, cancellationToken : token);
            }
        }

        private void CheckVisibility()
        {
            _isOffScreen = _camera.IsOffScreen(_enemyView.Bounds);

            if (_isOffScreen)
            {
                if (_despawnCts == null || _despawnCts.IsCancellationRequested)
                {
                    _despawnCts?.Dispose();
                    _despawnCts = new CancellationTokenSource();
                    DespawnAfterDelay(_despawnCts.Token).Forget();
                }
            }
            else
            {
                StopDespawnDelay();
            }
        }

        private async UniTaskVoid DespawnAfterDelay(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_model.EnemyDespawnDelay), cancellationToken: token);
                _onDespawn.OnNext(_enemyView);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                StopDespawnDelay();
            }
        }

        private void StopVisibilityCheckLoop()
        {
            _visibilityCts?.Cancel();
            _visibilityCts?.Dispose();
            _visibilityCts = null;
        }
        
        private void StopDespawnDelay()
        {
            _despawnCts?.Cancel();
            _despawnCts?.Dispose();
            _despawnCts = null;
        }

        // todo: this can be replaced with continuous collision in order to deal damage to the player so long they stay touching an enemy
        void OnGameObjectCollision(GameObject colliderObject)
        {
            var layer = colliderObject.layer;
            if (layer == LayerUtil.Player)
            {
                _gameEvents.PlayerHit(_model.EnemyDamage);
            }
            else if (layer == LayerUtil.Projectile)
            {
                _model.TakeDamage(1);
            }
        }
    }
}
