using UnityEngine;
using System.Collections;

namespace AntLion.Core
{
    public class HitFlash : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private float flashDuration = 0.08f;

        private Color originalColor;

        private void Awake(){
            originalColor = visual.color;
        }

        private void HandleDamaged(int amount)
        {
            StartCoroutine(Flash());
        }

        private void OnEnable()
        {
            health.OnDamaged += HandleDamaged;
        }

        private void OnDisable()
        {
            health.OnDamaged -= HandleDamaged;
        }

        private IEnumerator Flash()
        {
            visual.material.SetFloat("_FlashAmount", 1);
            visual.color = Color.white;
            yield return new WaitForSecondsRealtime(flashDuration);
            visual.material.SetFloat("_FlashAmount", 0);
            visual.color = originalColor;

        }
    }
    
}


