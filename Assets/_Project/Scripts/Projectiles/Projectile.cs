using AntLion.Core;
using UnityEngine;

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

        public float Speed => speed;
        public int Damage => damage;
        public float Lifetime => lifetime;

        private Rigidbody2D body;
        private ProjectilePool pool;
        private Vector2 direction;
        private float age;
        private bool returned;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        // Called once by the pool when this projectile is created.
        public void SetPool(ProjectilePool owner)
        {
            pool = owner;
        }

        // Places, aims and arms the projectile. Its layer comes from the prefab variant, not the caller.
        public void Launch(Vector2 position, Vector2 direction)
        {
            transform.position = position;
            body.position = position; // keep physics in step with the teleport, so it can't hit anything on the way
            this.direction = direction.normalized;
            age = 0f;
            returned = false;
        }

        private void FixedUpdate()
        {
            age += Time.fixedDeltaTime;

            // Bullets that never hit anything must still come back, or the pool drains during a held fire button.
            if (age >= lifetime)
            {
                ReturnToPool();
                return;
            }

            body.linearVelocity = motion != null
                ? motion.GetVelocity(direction, speed, age)
                : direction * speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (returned) return;

            // Health sits on the same GameObject as the collider on every damageable entity.
            // A target that ignores the hit (invulnerable, already dead) still consumes the bullet.
            if (other.TryGetComponent(out Health health)) health.TakeDamage(damage);

            ReturnToPool();
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
