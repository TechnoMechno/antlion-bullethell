using AntLion.Projectiles;
using UnityEngine;

namespace AntLion.Boss.Patterns
{
    // Everything a pattern needs to fire, passed in so patterns never search the scene.
    public struct AttackContext
    {
        public Transform FirePoint;
        public Transform Player;
        public ProjectilePool Pool;
    }
}
