using UnityEngine;
using UnityEngine.InputSystem;

namespace AntLion.Core
{
    // Step 6: Esc toggles pause. The pause rules themselves live in GameStateManager.
    public class PauseInput : MonoBehaviour
    {
        [SerializeField] private GameStateManager gameStateManager;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                gameStateManager.TogglePause();
            }
        }
    }
}
