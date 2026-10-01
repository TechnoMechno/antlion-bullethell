using System;
using System.Collections.Generic;
using AntLion.Boss.Patterns;
using AntLion.Core;
using UnityEngine;

namespace AntLion.Boss
{
    // The boss's decision-maker. Two things run side by side:
    //  - Circling: while alive, the boss keeps moving around the player at a set distance, sideways to the
    //    player's aim (the hardest direction to hit), and now and then pauses and reverses direction.
    //  - A finite state machine for its shooting rhythm: Recover -> Attack -> Recover ..., until Dead.
    // It decides what to do and when; BossMovement and BossAttack carry it out.
    public class BossBrain : MonoBehaviour
    {
        [Header("Body")]
        [SerializeField] private BossMovement movement;
        [SerializeField] private BossAttack attack;

        [Header("Attacks")]
        [Tooltip("Patterns the boss cycles through. Swapping this list is the phase 2 seam.")]
        [SerializeField] private List<AttackPattern> patterns = new List<AttackPattern>();
        [SerializeField] private PatternSelector selector = new PatternSelector();

        [Header("Circling")]
        [Tooltip("Speed while a pattern is firing. Higher is harder to hit.")]
        [SerializeField] private float attackMoveSpeed = 4.5f;
        [Tooltip("Speed during the pause between attacks. Keep it low: this is the player's window to land hits.")]
        [SerializeField] private float recoverMoveSpeed = 1f;
        [Tooltip("How far from the player the boss tries to stay.")]
        [SerializeField] private float preferredDistance = 5f;
        [Tooltip("How far around the player each move goes, in degrees.")]
        [SerializeField] private float stepAngle = 35f;
        [Tooltip("Chance, after each move, that the boss pauses and reverses direction.")]
        [SerializeField, Range(0f, 1f)] private float reverseChance = 0.3f;
        [Tooltip("How long the boss stops before reversing. This is the readable tell, and a moment it's easy to hit.")]
        [SerializeField] private float reversePause = 0.4f;
        [Tooltip("How far moves stay from the arena wall. Roughly the boss's radius.")]
        [SerializeField] private float edgeMargin = 0.9f;

        [Header("Timing")]
        [Tooltip("Pause after each attack. This is the player's window to shoot back.")]
        [SerializeField] private float recoverTime = 1.2f;

        [Header("Debug")]
        [SerializeField] private bool logStateChanges;

        // Presentation (animation, sound) can listen to this. The brain never touches visuals itself.
        public event Action<BossState> OnStateChanged;

        public BossMovement Movement => movement;
        public BossAttack Attack => attack;
        public float RecoverTime => recoverTime;

        public BossState Attacking { get; private set; }
        public BossState Recovering { get; private set; }
        public BossState Dead { get; private set; }
        public BossState Current { get; private set; }

        private Transform player;
        private ArenaBounds arena;
        private int circleDirection = 1;   // +1 counter-clockwise around the player, -1 clockwise
        private float pauseTimer;

        private void Awake()
        {
            Attacking = new AttackState(this);
            Recovering = new RecoverState(this);
            Dead = new DeadState(this);
            circleDirection = UnityEngine.Random.value < 0.5f ? 1 : -1;
        }

        // BossController, once.
        public void Init(Transform playerTransform, ArenaBounds arenaBounds)
        {
            player = playerTransform;
            arena = arenaBounds;
        }

        // Start with a short pause so the player gets a moment before the first attack.
        private void Start()
        {
            ChangeState(Recovering);
        }

        private void Update()
        {
            Current?.Tick();
            Circle();
        }

        public void ChangeState(BossState next)
        {
            if (next == Current) return;

            Current?.Exit();
            Current = next;
            if (logStateChanges) Debug.Log($"[BossBrain] {next.Name}", this);
            Current.Enter();
            OnStateChanged?.Invoke(Current);
        }

        // BossController calls this from Health.OnDeath.
        public void Die()
        {
            ChangeState(Dead);
        }

        public AttackPattern NextPattern()
        {
            return selector.Next(patterns);
        }

        // Keeps the boss circling the player for as long as it's alive, independent of the attack states.
        private void Circle()
        {
            if (Current == Dead || player == null || arena == null) return;

            // Standing still before a reversal.
            if (pauseTimer > 0f)
            {
                pauseTimer -= Time.deltaTime;
                return;
            }

            float speed = Current == Attacking ? attackMoveSpeed : recoverMoveSpeed;
            if (movement.IsMoving)
            {
                movement.SetSpeed(speed);   // speed changes as attacks start and end
                return;
            }

            // Arrived. Sometimes stop and turn around, so the player can't settle into leading the shot.
            if (UnityEngine.Random.value < reverseChance)
            {
                circleDirection = -circleDirection;
                pauseTimer = reversePause;
                return;
            }

            movement.MoveTo(NextCirclePoint(), speed);
        }

        // The next point around the player: one step along the circle, at the preferred distance, inside the arena.
        private Vector2 NextCirclePoint()
        {
            Vector2 center = player.position;
            Vector2 here = transform.position;
            Vector2 fromPlayer = here - center;
            float angle = fromPlayer.sqrMagnitude > 0.001f ? Mathf.Atan2(fromPlayer.y, fromPlayer.x) * Mathf.Rad2Deg : 90f;

            Vector2 point = PointAround(center, angle + circleDirection * stepAngle);

            // Pinned against the wall: going this way gets nowhere, so go the other way.
            if (Vector2.Distance(point, here) < 0.5f)
            {
                circleDirection = -circleDirection;
                point = PointAround(center, angle + circleDirection * stepAngle);
            }
            return point;
        }

        private Vector2 PointAround(Vector2 center, float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            Vector2 point = center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * preferredDistance;
            return arena.ClampInside(point, edgeMargin);
        }

        // Shows the circle the boss tries to keep around the player, in the Scene view when the boss is selected.
        private void OnDrawGizmosSelected()
        {
            if (player == null) return;
            Gizmos.color = Color.yellow;
            const int segments = 48;
            Vector2 center = player.position;
            Vector2 previous = center + Vector2.right * preferredDistance;
            for (int i = 1; i <= segments; i++)
            {
                float a = i * Mathf.PI * 2f / segments;
                Vector2 next = center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * preferredDistance;
                Gizmos.DrawLine(previous, next);
                previous = next;
            }
        }
    }
}
