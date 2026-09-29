using System.Collections;
using UnityEngine;

namespace AntLion.Boss.Patterns
{
    // Sinusoidal wave of bullets.
    [CreateAssetMenu(menuName = "AntLion/Attack Patterns/Sin Wave", fileName = "Pattern_SinWave")]
    public class SinWavePattern : AttackPattern
    {
        public override IEnumerator Execute(AttackContext context)
        {
            yield break;
        }
    }
}
