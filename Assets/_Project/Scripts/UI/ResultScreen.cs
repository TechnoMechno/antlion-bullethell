using AntLion.Core;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace AntLion.UI
{
    // Step 6: win or lose screen with Retry and Menu buttons. One script serves both scenes.
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text text;
        [SerializeField] private string menuScene = "MainMenu";

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Win || state == GameState.Lose)
            {
                panel.SetActive(true);
                if (state == GameState.Win)
                {
                    text.text = "You Win!";
                }
                else
                {
                    text.text = "You Lose!";
                }

            }
        }

        public void Retry()
        {
            sceneLoader.Load(SceneManager.GetActiveScene().name);
        }

        public void BackToMenu()
        {
            sceneLoader.Load(menuScene);
        }

        private void OnEnable()
        {
            // Add self to gameStateManager's list
            gameStateManager.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            gameStateManager.OnStateChanged -= HandleStateChanged;
        }
    }
}
