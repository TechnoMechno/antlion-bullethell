using AntLion.Core;
using UnityEngine;

namespace AntLion.Player
{
    public class PlayerKnockback : MonoBehaviour
    {
        [SerializeField] private Health hp;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private float knockbackSpeed = 8;
        [SerializeField] private float knockbackDuration = 0.1f;

        private Vector2 pushDirection;
        private float timer;

        private void HandleKnockback(int amount, Vector2 srcPos)
        {
            Vector2 direction = body.position - srcPos;
            if (direction.sqrMagnitude < 0.01f)
            {
                return;
            }
            pushDirection = direction.normalized;
            timer = knockbackDuration;
            movement.OverriddenDirection = true;
        }

        private void FixedUpdate()
        {
            if (timer <= 0f)
            {
                return;
            }
            timer -= Time.fixedDeltaTime;
            body.linearVelocity = pushDirection * knockbackSpeed;
            if (timer <= 0f)
            {
                movement.OverriddenDirection = false;            
            }

        }

        private void OnEnable()
        {
            hp.OnDamaged += HandleKnockback;
        }

        private void OnDisable()
        {
            hp.OnDamaged -= HandleKnockback;
        }
    }
}