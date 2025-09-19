using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Slider _healthBar;
        
        private RectTransform _healthBarRectTransform;
        
        public Bounds Bounds => GetFullBounds();
        
        private Bounds GetFullBounds()
        {
            var bounds = _spriteRenderer.bounds;

            Vector3[] corners = new Vector3[4];

            if (_healthBarRectTransform == null)
            {
                _healthBarRectTransform =  _healthBar.GetComponent<RectTransform>();
            }
            _healthBarRectTransform.GetWorldCorners(corners);
            Bounds healthBounds = new Bounds(corners[0], Vector3.zero);
            
            for (int i = 1; i < 4; i++)
            {
                healthBounds.Encapsulate(corners[i]);
            }

            bounds.Encapsulate(healthBounds);

            return bounds;
        }

    }
}