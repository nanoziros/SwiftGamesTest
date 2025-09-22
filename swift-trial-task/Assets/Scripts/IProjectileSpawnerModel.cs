namespace Scripts
{
    public interface IProjectileSpawnerModel
    {
        public int MaxProjectiles { get;  }
        public float InitialSpawnDelay { get;  }
        public float SpawnInterval { get;  }
    }
}