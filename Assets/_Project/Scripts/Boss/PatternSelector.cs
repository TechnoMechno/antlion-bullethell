using System.Collections.Generic;
using AntLion.Boss.Patterns;

namespace AntLion.Boss
{
    // Chooses which AttackPattern BossAttack runs next (in order, or random without repeats).
    [System.Serializable]
    public class PatternSelector
    {
        public AttackPattern Next(IReadOnlyList<AttackPattern> patterns)
        {
            throw new System.NotImplementedException();
        }
    }
}
