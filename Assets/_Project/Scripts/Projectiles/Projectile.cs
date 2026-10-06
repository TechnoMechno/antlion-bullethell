using UnityEngine;
using AntLion.Core;

namespace AntLion.Projectiles
{
    // The one bullet used by both sides. Each side fires a prefab variant (Projectile_Player / Projectile_Boss)
    // from its own pool; the variant carries its layer, and the collision matrix decides what that layer can hit,
    // so there is no player/boss bullet class.
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private int damage = 7;
        [SerializeField] private float lifetime = 3f;
        [Tooltip("Optional. Straight-line movement is used when this is empty.")]
        [SerializeField] private ProjectileMotion motion;
        
        private int currentDamage;
        private float currentSpeed;

        public float Speed => speed;
        public int Damage => damage;
        public float Lifetime => lifetime;

        private Rigidbody2D body;
        private Collider2D hitbox;
        private ProjectileHitEffect hitEffect; // optional; without one the bullet vanishes on hit
        private ProjectilePool pool;
        private Vector2 direction;
        private float age;
        private bool returned;
        private bool impacting; // hit something; standing still while the hit effect plays

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            hitbox = GetComponent<Collider2D>();
            hitEffect = GetComponentInChildren<ProjectileHitEffect>(true);
        }

        // Called once by the pool when this projectile is created.
        public void SetPool(ProjectilePool owner)
        {
            pool = owner;
        }

        // Places, aims and arms the projectile. Its layer comes from the prefab variant, not the caller.
        public void Launch(Vector2 position, Vector2 direction)
        {   
            currentDamage = damage;
            currentSpeed = speed;
            transform.position = position;
            body.position = position; // keep physics in step with the teleport, so it can't hit anything on the way
            this.direction = direction.normalized;
            age = 0f;
            returned = false;
            impacting = false;
            hitbox.enabled = true;
        }

        public void Launch(Vector2 position, Vector2 direction, int launchDamage, float launchSpeed)
        {
            Launch(position, direction);
            currentDamage = launchDamage;
            currentSpeed = launchSpeed;

        }

        private void FixedUpdate()
        {
            // After a hit: stay put until the hit effect has finished, then go back to the pool.
            if (impacting)
            {
                if (hitEffect.IsFinished) ReturnToPool();
                return;
            }

            age += Time.fixedDeltaTime;

            // Bullets that never hit anything must still come back, or the pool drains during a held fire button.
            if (age >= lifetime)
            {
                ReturnToPool();
                return;
            }

            body.linearVelocity = motion != null
                ? motion.GetVelocity(direction, currentSpeed, age)
                : direction * currentSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (returned || impacting) return;

            // Health sits on the same GameObject as the collider on every damageable entity.
            // A target that ignores the hit (invulnerable, already dead) still consumes the bullet.
            if (other.TryGetComponent(out Health health)) health.TakeDamage(currentDamage, transform.position);

            if (health != null && health.IsInvulnerable == true)
            {
                return;
            }
            Impact();
        }

        // Stops the bullet where it hit and plays its hit effect; FixedUpdate returns it once the effect ends.
        private void Impact()
        {
            if (hitEffect == null)
            {
                ReturnToPool();
                return;
            }

            impacting = true;
            hitbox.enabled = false; // can't hit anything else while the effect plays
            body.linearVelocity = Vector2.zero;
            hitEffect.Play();
        }

        private void ReturnToPool()
        {
            // Guard against returning twice, e.g. when two colliders are hit on the same frame.
            if (returned) return;
            returned = true;

            body.linearVelocity = Vector2.zero;
            pool.Return(this);
        }
    }
}
