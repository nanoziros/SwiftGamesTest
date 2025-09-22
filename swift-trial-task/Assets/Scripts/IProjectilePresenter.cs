using System;
using Zenject;

namespace Scripts
{
    public interface IProjectilePresenter
    {
        public void Initialize(AProjectileView view);
        public void Fire();
        
        public IObservable<AProjectileView> OnDespawn { get; }

        public class Factory : PlaceholderFactory<IProjectilePresenter> { }
    }
}