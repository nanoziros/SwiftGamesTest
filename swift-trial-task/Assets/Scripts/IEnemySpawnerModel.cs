namespace Scripts
{
    public interface IEnemySpawnerModel
    {
        public int MaxActiveEnemies { get; }
        public int EnemySpawnBatch { get; }
        public float SpawnInterval { get; }
    }
}