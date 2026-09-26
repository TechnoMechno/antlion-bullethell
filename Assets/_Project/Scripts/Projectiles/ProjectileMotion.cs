using UnityEngine;

namespace AntLion.Projectiles
{
    // How a projectile moves. One subclass per movement type.
    public abstract class ProjectileMotion : ScriptableObject
    {
        public abstract Vector2 GetVelocity(Vector2 direction, float speed, float age);
    }
}
