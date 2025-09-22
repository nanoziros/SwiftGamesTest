namespace Scripts
{
    public interface IProjectileSpawnerModel
    {
        public int MaxProjectiles { get;  }
        public float SpawnInterval { get;  }
    }
}