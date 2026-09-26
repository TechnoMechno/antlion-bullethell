using UnityEngine;

namespace AntLion.PowerUps
{
    // Step 5: the single pickup prefab. Applies its effect to the player on contact.
    public class PowerUpPickup : MonoBehaviour
    {
        [SerializeField] private PowerUpEffect effect;
    }
}
