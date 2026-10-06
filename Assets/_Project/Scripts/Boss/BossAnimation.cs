using UnityEngine;
using AntLion.Core;

namespace AntLion.Boss
{
    // Presentation only: reads what the boss's systems are doing and tells the Animator. Never changes gameplay.
    public class BossAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private BossMovement movement;
        [SerializeField] private BossAttack attack;
        [SerializeField] private Health health;

        // Animator parameter names, turned into numbers once so Unity doesn't compare strings every frame
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsShooting = Animator.StringToHash("IsShooting");
        private static readonly int Die = Animator.StringToHash("Die");

        private void OnEnable()
        {
            health.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            health.OnDeath -= HandleDeath;
        }

        private void Update()
        {
            if (health.IsDead) return;

            animator.SetBool(IsMoving, movement.IsMoving);
            animator.SetBool(IsShooting, attack.IsAttacking);
        }

        private void HandleDeath()
        {
            // The win screen sets Time.timeScale to 0. Run on real time so the death animation still plays.
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.SetBool(IsMoving, false);
            animator.SetBool(IsShooting, false);
            animator.SetTrigger(Die);
        }
    }
}
