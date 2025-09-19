namespace Scripts
{
    public class EnemySpawnerModel : IEnemySpawnerModel
    {
        public int MaxActiveEnemies => 20;
        public float SpawnInterval => 2;
    }
}