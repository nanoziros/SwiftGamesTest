using System;
using Scripts.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private RectTransform _healthBarRectTransform;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Slider _healthBar;
        
        private readonly Subject<Unit> _onEnabled = new();
        private readonly Subject<Unit> _onDisabled = new();
        
        public IObservable<Unit> OnEnabled => _onEnabled;
        public IObservable<Unit> OnDisabled => _onDisabled;
        
        public Bounds Bounds => GetFullBounds();
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        private void OnEnable()
        {
            _onEnabled.OnNext(Unit.Default);
        }

        private void OnDisable()
        {
            _onDisabled.OnNext(Unit.Default);
        }

        private Bounds GetFullBounds()
        {
            var bounds = _spriteRenderer.bounds;

            Vector3[] corners = new Vector3[4];
            _healthBarRectTransform.GetWorldCorners(corners);
            Bounds healthBounds = new(corners[0], Vector3.zero);
            
            for (int i = 1; i < 4; i++)
            {
                healthBounds.Encapsulate(corners[i]);
            }

            bounds.Encapsulate(healthBounds);

            return bounds;
        }
    }
}