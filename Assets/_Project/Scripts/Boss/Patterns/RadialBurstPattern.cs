using System.Collections;
using UnityEngine;

namespace AntLion.Boss.Patterns
{
    // Ring of bullets fired outward.
    [CreateAssetMenu(menuName = "AntLion/Attack Patterns/Radial Burst", fileName = "Pattern_RadialBurst")]
    public class RadialBurstPattern : AttackPattern
    {
        [SerializeField] private float windup = 0.5f;
        [SerializeField, Min(1)] private int rings = 4;
        [SerializeField, Min(1)] private int shotsPerRing = 20;
        [SerializeField] private float timeBetweenRings = 0.6f;
        [SerializeField, Range(0f, 1f)] private float ringOffset = 0.5f;

        public override IEnumerator Execute(AttackContext context)
        {
            yield return new WaitForSeconds(windup);
            float step = 360f/shotsPerRing;

            for (int i = 0; i < rings; i++)
            {
                FireRing(context, i * step * ringOffset, step);
                if (i < rings - 1)
                    yield return new WaitForSeconds(timeBetweenRings);

            }
        }

        private void FireRing(AttackContext context, float startAngle, float step)
        {
            Vector2 origin = context.FirePoint.position;

            for (int i = 0; i < shotsPerRing; i++)
            {
                float rad = (startAngle + step * i) * Mathf.Deg2Rad;
                context.Pool.Get().Launch(origin, new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)));
            }

        }
    }
}
