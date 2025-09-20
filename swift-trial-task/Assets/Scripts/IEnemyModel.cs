namespace Scripts
{
    public interface IEnemyModel
    {
        public float VisibilityCheckInterval { get; }
        public float EnemyDespawnDelay { get; }
    }
}