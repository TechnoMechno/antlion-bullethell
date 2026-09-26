using AntLion.Core;
using AntLion.Projectiles;
using UnityEngine;

namespace AntLion.Boss
{
    // Wires the boss's systems (Health, BossAttack) together. Holds no gameplay rules.
    public class BossController : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private BossAttack attack;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform player;
        [SerializeField] private ProjectilePool pool;
    }
}
