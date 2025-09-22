using UniRx;
using Zenject;

namespace Scripts
{
    public class PlayerPresenter : IInitializable, IPlayerHitReceiver
    {
        private readonly JoystickView _joystickView;
        private readonly PlayerView _playerView;
        private readonly IPlayerModel _playerModel;
        private readonly CompositeDisposable _disposer;
        public PlayerPresenter(JoystickView joystickView, PlayerView playerView, IPlayerModel playerModel, CompositeDisposable disposer)
        {
            _joystickView = joystickView;
            _playerView = playerView;
            _playerModel = playerModel;
            _disposer = disposer;
        }

        public void Initialize()
        {
            _joystickView.OnInput.Subscribe(_playerView.Move).AddTo(_disposer);
            _joystickView.OnInputEnd.Subscribe(_ => _playerView.OnMoveEnd()).AddTo(_disposer);
            _playerModel.CurrentHealth
                .Subscribe(currentHealth =>
                {
                    _playerView.UpdateHealth(currentHealth / _playerModel.MaxHealth);
                })
                .AddTo(_disposer);
        }
        
        public void ReceiveHit(float damage)
        {
            _playerModel.TakeDamage(damage);
        }
    }
}