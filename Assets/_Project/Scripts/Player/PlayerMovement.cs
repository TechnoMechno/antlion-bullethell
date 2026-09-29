using UnityEngine;

namespace AntLion.Player
{
    // Moves the player's Rigidbody2D in whatever direction it is given.
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;

        //For Movement
        private Rigidbody2D body;
        private Vector2 direction;

        //For Dash
        public Vector2 facingDirection {get; private set;} = Vector2.right;//Initial direction is right (1,0)
        public bool OverriddenDirection {get; set;}//Override the direction of the player to Dash

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void SetDirection(Vector2 newDirection)
        {
            direction = Vector2.ClampMagnitude(newDirection, 1f);

            //Set the facing direction for Dash
            if(newDirection.sqrMagnitude > 0.01f)
                facingDirection = newDirection.normalized;
        }

        // Move through the Rigidbody2D so physics stops the player at walls
        private void FixedUpdate()
        {
            if (OverriddenDirection) return; //If the player is dashing, don't move
            body.linearVelocity = direction * speed;
        }
    }
}
