using System.Collections;
using UnityEngine;

namespace AntLion.Boss.Patterns
{
    // Ring of bullets fired outward.
    [CreateAssetMenu(menuName = "AntLion/Attack Patterns/Radial Burst", fileName = "Pattern_RadialBurst")]
    public class RadialBurstPattern : AttackPattern
    {
        public override IEnumerator Execute(AttackContext context)
        {
            yield break;
        }
    }
}
