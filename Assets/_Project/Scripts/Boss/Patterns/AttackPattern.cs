using System.Collections;
using UnityEngine;

namespace AntLion.Boss.Patterns
{
    // Base class for every boss attack. One subclass per pattern; tuning lives on the asset.
    public abstract class AttackPattern : ScriptableObject
    {
        public abstract IEnumerator Execute(AttackContext context);
    }
}
