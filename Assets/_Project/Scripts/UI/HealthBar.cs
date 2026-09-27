using AntLion.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AntLion.UI
{
    // Step 2: fills a bar from any Health's OnChanged. Works for player and boss. Display only.
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Image fill;

        // Redraw on enable too, so a bar hidden while HP changed (e.g. HUD off during a pause) isn't stale.
        private void OnEnable()
        {
            health.OnChanged += Show;
            Show(health.Current, health.MaxHealth);
        }

        private void OnDisable()
        {
            health.OnChanged -= Show;
        }

        // Health sets its starting HP in Awake, which may run after this bar's first OnEnable, so draw again here.
        private void Start()
        {
            Show(health.Current, health.MaxHealth);
        }

        private void Show(int current, int max)
        {
            fill.fillAmount = max > 0 ? (float)current / max : 0f;
        }
    }
}
