using UnityEngine;

namespace AntLion.Player
{
    // Moves the player's Rigidbody2D in whatever direction it is given.
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;

        private Rigidbody2D body;
        private Vector2 direction;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void SetDirection(Vector2 newDirection)
        {
            direction = Vector2.ClampMagnitude(newDirection, 1f);
        }

        // Move through the Rigidbody2D so physics stops the player at walls
        private void FixedUpdate()
        {
            body.linearVelocity = direction * speed;
        }
    }
}
