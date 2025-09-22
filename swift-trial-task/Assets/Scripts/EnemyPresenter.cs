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
        private readonly PlayerView _playerView;
        private readonly EnemyView _enemyView;
        private readonly IEnemyModel _model;
        private readonly Camera _camera;
        private bool _isOffScreen = true;
        private CancellationTokenSource _visibilityCts;
        private CancellationTokenSource _despawnCts;
        
        private readonly Subject<EnemyView> _onDespawn = new();
        public IObservable<EnemyView> OnDespawn => _onDespawn;
        public bool IsOffScreen => _isOffScreen;
        public Vector2 Position => _enemyView.Position;

        public EnemyPresenter(EnemyView enemyEnemyView, PlayerView playerView, IEnemyModel model, Camera camera, CompositeDisposable disposer)
        {
            _playerView = playerView;
            _enemyView = enemyEnemyView;
            _model = model;
            _camera = camera;

            StartVisibilityLoop();
            
            _enemyView.OnEnabled.Subscribe(_=> StartVisibilityLoop()).AddTo(disposer);
            _enemyView.OnDisabled.Subscribe(_ => StopVisibilityLoop()).AddTo(disposer);
        }

        public void SetRandomOffScreenPosition()
        {
            var targetPosition = _camera.GetRandomOffScreenPosition(_enemyView.Bounds, _playerView.Velocity);
            
            // todo: also guarantee the enemy is within the map bounds
            _enemyView.SetPosition(targetPosition);
        }

        private void StartVisibilityLoop()
        {
            if (_visibilityCts is { IsCancellationRequested: false })
            {
                return;
            }

            _visibilityCts?.Dispose();
            _visibilityCts = new CancellationTokenSource();
            VisibilityLoop(_visibilityCts.Token).Forget();
        }

        private void StopVisibilityLoop()
        {
            _visibilityCts?.Cancel();
            _visibilityCts?.Dispose();
            _visibilityCts = null;
        }
        
        private async UniTaskVoid VisibilityLoop(CancellationToken token)
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
                _despawnCts?.Cancel();
                _despawnCts?.Dispose();
                _despawnCts = null;
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
                if (_despawnCts != null)
                {
                    _despawnCts.Dispose();
                    _despawnCts = null;
                }
            }
        }
    }
}
