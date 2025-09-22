namespace Scripts
{
    public class CrossbowSpawnerModel : IProjectileSpawnerModel
    {
        public int MaxProjectiles => 1;
        public float SpawnInterval => 2;
    }
}