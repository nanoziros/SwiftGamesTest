namespace Scripts
{
    public class CrossbowSpawnerModel : IProjectileSpawnerModel
    {
        public int MaxProjectiles => 5;
        public float SpawnInterval => 5;
        public float InitialSpawnDelay => 1;
    }
}