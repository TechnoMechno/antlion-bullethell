using UnityEngine;

namespace AntLion.PowerUps
{
    // Step 5: base for every power-up. Adding a power-up means authoring an asset of a subclass.
    public abstract class PowerUpEffect : ScriptableObject
    {
        public abstract void Apply(GameObject target);
    }
}
