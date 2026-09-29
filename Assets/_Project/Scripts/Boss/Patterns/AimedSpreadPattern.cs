using System.Collections;
using UnityEngine;

namespace AntLion.Boss.Patterns
{
    // Fan of bullets aimed at the player, fired in volleys. With 1 bullet per volley it is a plain aimed shot.
    [CreateAssetMenu(menuName = "AntLion/Attack Patterns/Aimed Spread", fileName = "Pattern_AimedSpread")]
    public class AimedSpreadPattern : AttackPattern
    {
        [Tooltip("Pause before the first volley, so the player can see the attack coming.")]
        [SerializeField] private float windup = 0.5f;
        [SerializeField, Min(1)] private int volleys = 3;
        [SerializeField, Min(1)] private int bulletsPerVolley = 5;
        [Tooltip("Total width of the fan in degrees. Ignored when there is only one bullet.")]
        [SerializeField] private float spreadAngle = 40f;
        [SerializeField] private float timeBetweenVolleys = 0.6f;

        public override IEnumerator Execute(AttackContext context)
        {
            yield return new WaitForSeconds(windup);

            for (int v = 0; v < volleys; v++)
            {
                FireVolley(context);

                // No wait after the last volley: the pattern ends and the brain decides what's next.
                if (v < volleys - 1) yield return new WaitForSeconds(timeBetweenVolleys);
            }
        }

        private void FireVolley(AttackContext context)
        {
            Vector2 origin = context.FirePoint.position;
            // Re-aimed every volley, so standing still gets punished but moving is always an escape.
            Vector2 toPlayer = ((Vector2)context.Player.position - origin).normalized;

            for (int i = 0; i < bulletsPerVolley; i++)
            {
                // Spread the bullets evenly from -spreadAngle/2 to +spreadAngle/2; a single bullet goes dead center.
                float t = bulletsPerVolley == 1 ? 0.5f : i / (bulletsPerVolley - 1f);
                float angle = Mathf.Lerp(-spreadAngle / 2f, spreadAngle / 2f, t);
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * toPlayer;

                context.Pool.Get().Launch(origin, direction);
            }
        }
    }
}
