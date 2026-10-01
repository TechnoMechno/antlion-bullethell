using AntLion.Boss.Patterns;
using AntLion.Core;
using AntLion.Projectiles;
using UnityEngine;

namespace AntLion.Boss
{
    // Wires the boss's systems (Health, BossBrain, BossAttack) together. Holds no gameplay rules.
    public class BossController : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private BossBrain brain;
        [SerializeField] private BossAttack attack;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform player;
        [SerializeField] private ProjectilePool pool;
        [Tooltip("Optional. When empty, the scene's ArenaBounds is found at startup, so scenes don't need wiring.")]
        [SerializeField] private ArenaBounds arena;

        // Awake, so everything is ready before the brain's first state runs in Start.
        private void Awake()
        {
            if (arena == null) arena = FindAnyObjectByType<ArenaBounds>();
            if (arena == null) Debug.LogWarning("BossController found no ArenaBounds in the scene; the boss won't move.", this);

            attack.Init(new AttackContext { FirePoint = firePoint, Player = player, Pool = pool });
            brain.Init(player, arena);
        }

        private void OnEnable()
        {
            health.OnDeath += brain.Die;
        }

        private void OnDisable()
        {
            health.OnDeath -= brain.Die;
        }
    }
}
