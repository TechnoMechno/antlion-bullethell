using System.Collections;
using UnityEngine;

namespace AntLion.Boss.Patterns
{
    // Rotating stream of bullets.
    [CreateAssetMenu(menuName = "AntLion/Attack Patterns/Spiral", fileName = "Pattern_Spiral")]
    public class SpiralPattern : AttackPattern
    {
        public override IEnumerator Execute(AttackContext context)
        {
            yield break;
        }
    }
}
