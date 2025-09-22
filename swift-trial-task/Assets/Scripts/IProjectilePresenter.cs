using Zenject;

namespace Scripts
{
    public interface IProjectilePresenter
    {
        public void Fire();
        
        public class Factory : PlaceholderFactory<IProjectilePresenter> { }
    }
}