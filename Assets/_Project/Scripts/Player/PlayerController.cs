using UnityEngine;
using UnityEngine.InputSystem;

namespace AntLion.Player
{
    // Reads input and hands it to the player's systems. Holds no gameplay rules.
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerShooter shooter;

        private InputAction moveAction;

        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
        }

        // Read input every frame so no key press is missed
        private void Update()
        {
            movement.SetDirection(moveAction.ReadValue<Vector2>());
            // Step 1: read the Attack action and call shooter.Fire(direction) once aiming is decided.
        }
    }
}
