using System.Collections;
using UnityEngine;

namespace AntLion.Boss.Patterns
{
    // Rotating stream of bullets.
    [CreateAssetMenu(menuName = "AntLion/Attack Patterns/Spiral", fileName = "Pattern_Spiral")]
    public class SpiralPattern : AttackPattern
    {
        [SerializeField] private float windup = 0.5f;
        [SerializeField, Min(1)] private int shots = 72;
        [SerializeField] private float timeBetweenShots = 0.12f;
        [SerializeField, Min(1)] private int arms = 2;
        [SerializeField] private float degreesPerShot = 10;

        public override IEnumerator Execute(AttackContext context)
        {
            //Pause for half a second before firing
            yield return new WaitForSeconds(windup);
            float angle = 0f;

            //Fire each shot
            for (int i = 0; i < shots; i++){
                Vector2 origin = context.FirePoint.position;
                for (int j = 0; j < arms; j++){
                    //Fire each arm
                    float rad = (angle + 360f / arms * j) * Mathf.Deg2Rad;
                    context.Pool.Get().Launch(origin, new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)));

                    }
                    //Change the angle for the next shot
                    angle = (angle + degreesPerShot) % 360f;
                    if (i < shots - 1){
                        yield return new WaitForSeconds(timeBetweenShots);
                }
            }
        }
    }
}
