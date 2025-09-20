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
        private readonly GameObjectPool<EnemyView> _pool;

        private CancellationTokenSource _visibilityCts;
        private CancellationTokenSource _despawnCts;

        public EnemyPresenter(EnemyView enemyEnemyView, PlayerView playerView, IEnemyModel model, Camera camera, GameObjectPool<EnemyView> pool, CompositeDisposable disposer)
        {
            _playerView = playerView;
            _enemyView = enemyEnemyView;
            _model = model;
            _camera = camera;
            _pool = pool;

            StartVisibilityLoop();
            
            _enemyView.OnEnabled.Subscribe(_=> StartVisibilityLoop()).AddTo(disposer);
            _enemyView.OnDisabled.Subscribe(_ => StopVisibilityLoop()).AddTo(disposer);
        }

        public void SetRandomOffScreenState()
        {
            _enemyView.SetRandomOffScreenState(_playerView.Velocity, _camera);
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
            bool offScreen = _camera.IsOffScreen(_enemyView.Bounds);

            if (offScreen)
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
                _pool.Return(_enemyView);
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
