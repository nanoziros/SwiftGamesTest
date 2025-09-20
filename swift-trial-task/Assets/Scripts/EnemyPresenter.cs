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
        private readonly EnemyView _view;
        private readonly IEnemyModel _model;
        private readonly Camera _camera;
        private readonly GameObjectPool<EnemyView> _pool;

        private CancellationTokenSource _visibilityCts;
        private CancellationTokenSource _despawnCts;

        public EnemyPresenter(EnemyView enemyView, IEnemyModel model, Camera camera, GameObjectPool<EnemyView> pool, CompositeDisposable disposer)
        {
            _view = enemyView;
            _model = model;
            _camera = camera;
            _pool = pool;

            StartVisibilityLoop();
            
            _view.OnEnabled.Subscribe(_=> StartVisibilityLoop()).AddTo(disposer);
            _view.OnDisabled.Subscribe(_ => StopVisibilityLoop()).AddTo(disposer);
        }

        private void StartVisibilityLoop()
        {
            if (_visibilityCts == null || _visibilityCts.IsCancellationRequested)
            {
                _visibilityCts = new CancellationTokenSource();
                VisibilityLoop(_visibilityCts.Token).Forget();
            }
        }

        private void StopVisibilityLoop()
        {
            _visibilityCts?.Cancel();
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
            bool offScreen = _camera.IsOffScreen(_view.Bounds);

            if (offScreen)
            {
                if (_despawnCts == null || _despawnCts.IsCancellationRequested)
                {
                    _despawnCts = new CancellationTokenSource();
                    DespawnAfterDelay(_despawnCts.Token).Forget();
                }
            }
            else
            {
                _despawnCts?.Cancel();
            }
        }

        private async UniTaskVoid DespawnAfterDelay(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_model.EnemyDespawnDelay), cancellationToken: token);
                _pool.Return(_view);
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
