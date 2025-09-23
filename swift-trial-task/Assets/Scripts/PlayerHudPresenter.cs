using UniRx;
using Zenject;

namespace Scripts
{
    public class PlayerHudPresenter : IInitializable
    {
        private readonly IPlayerHudModel _model;
        private readonly PlayerHudView _view;
        private readonly IGameEvents _events;
        private readonly CompositeDisposable _disposer;

        public PlayerHudPresenter(IPlayerHudModel model, PlayerHudView view, IGameEvents gameEvents, CompositeDisposable disposer)
        {
            _model = model;
            _view = view;
            _disposer = disposer;
            _events = gameEvents;
        }
        
        public void Initialize()
        {
            _model.CurrentEnemiesDefeated.Subscribe(_view.SetEnemyKillCount).AddTo(_disposer);
            _events.OnEnemyKilled
                .Subscribe(_ => _model.SetEnemyKilled())
                .AddTo(_disposer);
        }
    }
}