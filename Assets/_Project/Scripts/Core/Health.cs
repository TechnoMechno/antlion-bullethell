using System;
using UnityEngine;

namespace AntLion.Core
{
    // Step 2: shared by player and boss. Listeners never need to know which entity it belongs to.
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 10;

#pragma warning disable CS0067 // raised once TakeDamage/Heal are implemented
        public event Action<int, int> OnChanged;
        public event Action OnDeath;
#pragma warning restore CS0067

        public int MaxHealth => maxHealth;

        public void TakeDamage(int amount)
        {
        }

        public void Heal(int amount)
        {
        }
    }
}
