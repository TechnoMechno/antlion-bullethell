using UnityEngine;

namespace AntLion.Projectiles
{
    // Constant velocity in the launch direction.
    [CreateAssetMenu(menuName = "AntLion/Projectile Motion/Straight", fileName = "Motion_Straight")]
    public class StraightMotion : ProjectileMotion
    {
        public override Vector2 GetVelocity(Vector2 direction, float speed, float age)
        {
            return direction * speed;
        }
    }
}
