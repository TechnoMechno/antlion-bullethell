using UnityEngine;

namespace AntLion.Projectiles
{
    // Step 1: the one bullet prefab for player and boss. Its layer (PlayerProjectile / BossProjectile) decides what it can hit.
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private ProjectileMotion motion;

        public float Speed => speed;
        public int Damage => damage;
        public float Lifetime => lifetime;

        public void Launch(Vector2 direction)
        {
        }
    }
}
