using System;
using UniRx;
using UnityEngine;

namespace Scripts
{
    public class CrossbowBoltPresenter : IProjectilePresenter
    {
        private readonly CrossbowBoltView _view;
        private readonly PlayerView _playerView;
        private readonly ICrossbowBoltModel _crossbowBoltModel;
        private readonly Camera _camera;
        
        private readonly Subject<CrossbowBoltView> _onDespawn = new();
        public IObservable<CrossbowBoltView> OnDespawn => _onDespawn;

        public CrossbowBoltPresenter(AProjectileView view, PlayerView playerView, ICrossbowBoltModel model, Camera camera)
        {
            _view = (CrossbowBoltView) view;
            _playerView = playerView;
            _crossbowBoltModel = model;
            _camera = camera;
        }
        
        public void Fire()
        {
            
        }
    }
}