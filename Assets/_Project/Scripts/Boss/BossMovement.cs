using UnityEngine;

namespace AntLion.Boss
{
    // Moves the boss's kinematic Rigidbody2D in a straight line to a target. Makes no decisions:
    // the brain picks where to go. Kinematic bodies pass through walls, so only give it points inside the arena.
    [RequireComponent(typeof(Rigidbody2D))]
    public class BossMovement : MonoBehaviour
    {
        public bool IsMoving { get; private set; }   // brain polls this to know when the boss has arrived

        private Rigidbody2D body;
        private Vector2 target;
        private float speed;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        // Brain: start moving. Replaces any move still in progress.
        public void MoveTo(Vector2 point, float moveSpeed)
        {
            target = point;
            speed = moveSpeed;
            IsMoving = true;
        }

        // Brain: change speed without changing where it's going (e.g. slow down while attacking).
        public void SetSpeed(float moveSpeed)
        {
            speed = moveSpeed;
        }

        // Brain: stop where it is.
        public void Stop()
        {
            IsMoving = false;
        }

        private void FixedUpdate()
        {
            if (!IsMoving) return;

            Vector2 next = Vector2.MoveTowards(body.position, target, speed * Time.fixedDeltaTime);
            body.MovePosition(next); // the correct way to move a kinematic body; keeps the collider with the sprite
            if (next == target) IsMoving = false;
        }
    }
}
