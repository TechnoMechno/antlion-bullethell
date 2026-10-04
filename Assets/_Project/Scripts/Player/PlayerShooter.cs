using AntLion.Projectiles;
using UnityEngine;

namespace AntLion.Player
{
    // Step 1: takes a projectile from the pool at the fire point and launches it.
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private ProjectilePool pool;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireInterval = 0.05f;
        [SerializeField] private PlayerStats stats;
        private float damageMultiplier = 1f;

        private float nextFireTime;

        public void SetDamageMultiplier(float multiplier)
        {
            damageMultiplier = multiplier;
        }

        public void Fire(Vector2 direction)
        {
            //Check if the player can fire
            if (Time.time < nextFireTime) return;
            nextFireTime = Time.time + fireInterval;

            int damage = Mathf.RoundToInt(stats.BulletDamage * damageMultiplier);
            float speed = stats.BulletSpeed;

            //Shoot the projectile
            pool.Get().Launch(firePoint.position, direction, damage, speed);
        }
    }
}
