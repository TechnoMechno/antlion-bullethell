using AntLion.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AntLion.UI
{
    // Step 2: fills a bar from any Health's OnChanged. Works for player and boss. Display only.
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Image fill;
    }
}
