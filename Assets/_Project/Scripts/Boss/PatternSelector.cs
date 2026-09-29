using System.Collections.Generic;
using AntLion.Boss.Patterns;

namespace AntLion.Boss
{
    // Chooses which AttackPattern the boss runs next. Goes through the list in order and wraps around.
    // Random-without-repeats can be added as a mode once there are enough patterns to need it.
    [System.Serializable]
    public class PatternSelector
    {
        private int index = -1;

        // Returns null when the list is empty, so the caller can warn instead of crashing.
        public AttackPattern Next(IReadOnlyList<AttackPattern> patterns)
        {
            if (patterns == null || patterns.Count == 0) return null;

            index = (index + 1) % patterns.Count;
            return patterns[index];
        }
    }
}
