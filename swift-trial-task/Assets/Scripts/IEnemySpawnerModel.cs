namespace Scripts
{
    public interface IEnemySpawnerModel
    {
        public int MaxActiveEnemies { get; }
        public float SpawnInterval { get; }
    }
}