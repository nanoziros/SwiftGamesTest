namespace Scripts
{
    public class CrossbowBoltModel : ICrossbowBoltModel
    {
        public float InitialBiasTowardsEnemy => 0.75f;
        public float AfterBounceBiasTowardsEnemy => 1f;
        public float MaxDistance => 100;
    }
}