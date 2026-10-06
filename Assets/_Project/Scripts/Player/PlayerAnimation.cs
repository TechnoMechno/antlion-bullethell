using UnityEngine;
using AntLion.Core;

namespace AntLion.Player
{
    // Presentation only: reads what the player's systems are doing and tells the Animator. Never changes gameplay.
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private PlayerDash dash;
        [SerializeField] private PlayerShooter shooter;
        [SerializeField] private Health health;

        [Tooltip("How long the shoot animation keeps playing after the shoot button is released.")]
        [SerializeField] private float shootHoldTime = 0.1f;

        [Tooltip("How long the hurt animation replaces idle/move after being hit.")]
        [SerializeField] private float hurtDuration = 0.5f;

        // Animator parameter names, turned into numbers once so Unity doesn't compare strings every frame
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsShooting = Animator.StringToHash("IsShooting");
        private static readonly int IsDashing = Animator.StringToHash("IsDashing");
        private static readonly int DashDir = Animator.StringToHash("DashDir");
        private static readonly int Die = Animator.StringToHash("Die");
        private static readonly int IsHurt = Animator.StringToHash("IsHurt");

        // Values for DashDir; must match the conditions in visual.controller
        private const int DashUp = 0;
        private const int DashDown = 1;
        private const int DashLeft = 2;
        private const int DashRight = 3;

        // Time.time at which the hurt animation stops
        private float hurtUntil;

        private void OnEnable()
        {
            health.OnDeath += HandleDeath;
            health.OnDamaged += HandleDamaged;
        }

        private void OnDisable()
        {
            health.OnDeath -= HandleDeath;
            health.OnDamaged -= HandleDamaged;
        }

        private void Update()
        {
            if (health.IsDead) return;

            bool isDashing = dash.State == DashState.Dashing;

            // Set the direction before IsDashing so the right dash clip is chosen on the first frame
            if (isDashing)
                animator.SetInteger(DashDir, ToDashDir(dash.DashDirection));

            animator.SetBool(IsDashing, isDashing);
            animator.SetBool(IsMoving, body.linearVelocity.sqrMagnitude > 0.01f);
            animator.SetBool(IsShooting, Time.time - shooter.LastTriggerTime < shootHoldTime);
            animator.SetBool(IsHurt, Time.time < hurtUntil);
        }

        // Each hit restarts the hurt timer
        private void HandleDamaged(int amount, Vector2 sourcePosition)
        {
            hurtUntil = Time.time + hurtDuration;
        }

        // Picks the closest of the four dash clips: whichever axis the dash leans on more wins
        private static int ToDashDir(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                return direction.x > 0f ? DashRight : DashLeft;
            return direction.y > 0f ? DashUp : DashDown;
        }

        private void HandleDeath()
        {
            // The lose screen sets Time.timeScale to 0. Run on real time so the death animation still plays.
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            // Clear the other switches so no dash/move arrow can pull the player out of the death animation
            animator.SetBool(IsDashing, false);
            animator.SetBool(IsMoving, false);
            animator.SetBool(IsShooting, false);
            animator.SetBool(IsHurt, false);
            animator.SetTrigger(Die);
        }
    }
}
