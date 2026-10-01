using System;
using UnityEngine;

namespace AntLion.Core
{
    // Tracks Playing / Win / Lose from both OnDeath events and drives transitions. Keep it small.
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;
        [SerializeField] private Health bossHealth;
        [SerializeField] private SceneLoader sceneLoader;

        public event Action<GameState> OnStateChanged;

        public GameState State { get; private set; } = GameState.Playing;

        [ContextMenu("pause")]
        public void TogglePause()
        {
            if (State == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
            else if (State == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }

        private void HandleBossDeath()
        {
            SetState(GameState.Win);
        }

        private void HandlePlayerDeath()
        {
            SetState(GameState.Lose);
        }

        private void OnEnable()
        {
            bossHealth.OnDeath += HandleBossDeath;
            playerHealth.OnDeath += HandlePlayerDeath;
        }

        private void OnDisable()
        {
            bossHealth.OnDeath -= HandleBossDeath;
            playerHealth.OnDeath -= HandlePlayerDeath;
        }

        private void SetState(GameState nextState)
        {
            if (State == GameState.Lose || State == GameState.Win)
            {
                return;
            }
            State = nextState;
            if (State == GameState.Lose || State == GameState.Win || State == GameState.Paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
            OnStateChanged?.Invoke(State);
            Debug.Log(State);
        }


    }
}
