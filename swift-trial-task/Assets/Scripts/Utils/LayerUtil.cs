using UnityEngine;

namespace Scripts.Utils
{
    // todo: to further reduce hardcode these layers can be stored in a scriptable object
    public static class LayerUtil
    {
        public static readonly int Player = LayerMask.NameToLayer("player");
        public static readonly int Projectile = LayerMask.NameToLayer("projectile");
    }
}