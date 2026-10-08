using AntLion.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AntLion.UI
{
    // Step 6: pause button in the corner while playing; faded screen with a centered resume button while paused.
    // Display only; GameStateManager does the pausing.
    public class PauseButton : MonoBehaviour
    {
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private GameObject playingView;
        [SerializeField] private GameObject pausedView;

        // Called by both buttons' OnClick.
        public void TogglePause()
        {
            gameStateManager.TogglePause();

            // Deselect the button, otherwise Space (dash) would also press it.
            EventSystem.current.SetSelectedGameObject(null);
        }

        private void HandleStateChanged(GameState state)
        {
            // Both views stay hidden once the fight is over; the result screen takes over.
            playingView.SetActive(state == GameState.Playing);
            pausedView.SetActive(state == GameState.Paused);
        }

        private void OnEnable()
        {
            gameStateManager.OnStateChanged += HandleStateChanged;
            HandleStateChanged(gameStateManager.State);
        }

        private void OnDisable()
        {
            gameStateManager.OnStateChanged -= HandleStateChanged;
        }
    }
}
