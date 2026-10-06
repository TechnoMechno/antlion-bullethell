using UnityEngine;

namespace AntLion.Projectiles
{
    // Presentation only. Shows the flight sprite while the bullet flies, and plays the Animator's hit clip once when
    // Projectile reports a hit. Projectile waits for IsFinished before returning the bullet to its pool.
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class ProjectileHitEffect : MonoBehaviour
    {
        [Tooltip("What the bullet looks like while flying. The Animator only runs after a hit.")]
        [SerializeField] private Sprite flightSprite;

        private Animator animator;
        private SpriteRenderer spriteRenderer;

        // True once the hit clip has played to its end (the clip must have Loop Time off).
        public bool IsFinished => animator.enabled && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // The pool re-activates bullets, so this runs every time one is reused.
        private void OnEnable()
        {
            animator.enabled = false;
            spriteRenderer.sprite = flightSprite;
        }

        public void Play()
        {
            animator.enabled = true;
            animator.Rebind();    // back to the start of the hit clip, even if this bullet played it before
            animator.Update(0f);  // show the first frame now instead of next frame
        }
    }
}
