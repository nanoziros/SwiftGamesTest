using System.Collections.Generic;

namespace Scripts
{
    public interface IEnemyProvider
    {
        IReadOnlyList<EnemyPresenter> GetVisibleEnemies();
    }
}