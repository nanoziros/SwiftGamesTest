namespace Scripts
{
    public class EnemySpawnerModel : IEnemySpawnerModel
    {
        public int MaxActiveEnemies => 30;
        public float SpawnInterval => 2;
        public int EnemySpawnBatch => 3;
    }
}