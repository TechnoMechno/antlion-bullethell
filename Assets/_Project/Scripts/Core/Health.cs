using System;
using UnityEngine;

namespace AntLion.Core
{
    // Shared by player and boss. Listeners never need to know which entity it belongs to.
    // Must sit on the same GameObject as the collider, because Projectile looks it up from what it hits.
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 10;

        public event Action<int, int> OnChanged;
        public event Action OnDeath;

        public int MaxHealth => maxHealth;
        public int Current { get; private set; }
        public bool IsDead { get; private set; }

        // Damage is ignored while this is true. The dash and the shield power-up switch it on and off;
        // they own the timing, this component only enforces the rule.
        public bool IsInvulnerable { get; private set; }

        private void Awake()
        {
            Current = maxHealth;
        }

        public void SetInvulnerable(bool value)
        {
            IsInvulnerable = value;
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0 || IsDead || IsInvulnerable) return;

            Current = Mathf.Max(0, Current - amount);
            OnChanged?.Invoke(Current, maxHealth);

            // IsDead guards against OnDeath firing twice, which would trigger two scene transitions.
            if (Current == 0)
            {
                IsDead = true;
                OnDeath?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead) return;

            Current = Mathf.Min(maxHealth, Current + amount);
            OnChanged?.Invoke(Current, maxHealth);
        }
    }
}
