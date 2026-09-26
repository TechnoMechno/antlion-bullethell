using System.Collections.Generic;
using UnityEngine;

namespace AntLion.PowerUps
{
    // Step 5: spawns pickups from a list of effect assets. Never needs editing to add a power-up.
    public class PowerUpSpawner : MonoBehaviour
    {
        [SerializeField] private PowerUpPickup pickupPrefab;
        [SerializeField] private List<PowerUpEffect> effects = new List<PowerUpEffect>();
    }
}
