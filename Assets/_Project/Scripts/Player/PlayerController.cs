using UnityEngine;
using UnityEngine.InputSystem;

namespace AntLion.Player
{
    // Reads input and hands it to the player's systems. Holds no gameplay rules.
    public class PlayerController : MonoBehaviour
    {
        private InputAction dashAction;
        private InputAction moveAction;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerShooter shooter;
        [SerializeField] private PlayerDash dash;

        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            dashAction = InputSystem.actions.FindAction("Dash");
        }

        // Read input every frame so no key press is missed
        private void Update()
        {
            //Set up Move Action
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            movement.SetDirection(moveInput);

            //Set up Dash Action
            if(dashAction.WasPressedThisFrame())
                dash.Dash(moveInput);
            
        }
    }
}
