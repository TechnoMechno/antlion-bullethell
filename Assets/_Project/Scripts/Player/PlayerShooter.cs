using AntLion.Projectiles;
using UnityEngine;

namespace AntLion.Player
{
    // Step 1: takes a projectile from the pool at the fire point and launches it.
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private ProjectilePool pool;
        [SerializeField] private Transform firePoint;

        public void Fire(Vector2 direction)
        {
        }
    }
}
