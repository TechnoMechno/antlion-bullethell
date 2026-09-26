using System.Collections;
using UnityEngine;

namespace AntLion.Boss.Patterns
{
    // Fan of bullets aimed at the player.
    [CreateAssetMenu(menuName = "AntLion/Attack Patterns/Aimed Spread", fileName = "Pattern_AimedSpread")]
    public class AimedSpreadPattern : AttackPattern
    {
        public override IEnumerator Execute(AttackContext context)
        {
            yield break;
        }
    }
}
