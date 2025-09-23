using UniRx;
using Zenject;

namespace Scripts
{
    public class PlayerPresenter : IInitializable
    {
        private readonly IGameEvents _gameEvents;
        private readonly JoystickView _joystickView;
        private readonly PlayerView _playerView;
        private readonly IPlayerModel _playerModel;
        private readonly CompositeDisposable _disposer;
        public PlayerPresenter(JoystickView joystickView, PlayerView playerView, IPlayerModel playerModel, IGameEvents gameEvents, CompositeDisposable disposer)
        {
            _joystickView = joystickView;
            _playerView = playerView;
            _playerModel = playerModel;
            _disposer = disposer;
            _gameEvents = gameEvents;
        }

        public void Initialize()
        {
            _joystickView.OnInput.Subscribe(_playerView.Move).AddTo(_disposer);
            _joystickView.OnInputEnd.Subscribe(_ => _playerView.OnMoveEnd()).AddTo(_disposer);
            
            _gameEvents.OnPlayerHit
                .Subscribe(_playerModel.TakeDamage)
                .AddTo(_disposer);
            _playerModel.CurrentHealth
                .Subscribe(currentHealth =>
                {
                    _playerView.UpdateHealth(currentHealth / _playerModel.MaxHealth);
                })
                .AddTo(_disposer);
        }
    }
}