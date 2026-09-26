using System.Collections.Generic;
using AntLion.Boss.Patterns;
using UnityEngine;

namespace AntLion.Boss
{
    // Step 3: runs one hardcoded pattern.
    // Step 4: runs whichever pattern in the list the PatternSelector picks. Keep the list swappable (phase 2 seam).
    public class BossAttack : MonoBehaviour
    {
        [SerializeField] private List<AttackPattern> patterns = new List<AttackPattern>();
        [SerializeField] private PatternSelector selector = new PatternSelector();
    }
}
