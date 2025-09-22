namespace Scripts
{
    public interface ICrossbowBoltModel
    {
        public float InitialBiasTowardsEnemy { get; }
        public float AfterBounceBiasTowardsEnemy { get; }
        public float MaxDistance { get; }
    }
}