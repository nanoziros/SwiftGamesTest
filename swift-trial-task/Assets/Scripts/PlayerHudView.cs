using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class PlayerHudView : MonoBehaviour
    {
        [SerializeField] private Text _enemyKillCount;

        public void SetEnemyKillCount(int count)
        {
            _enemyKillCount.text = $"Enemies Defeated: {count}";
        }
    }
}