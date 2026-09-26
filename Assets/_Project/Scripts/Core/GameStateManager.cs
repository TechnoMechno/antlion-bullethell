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

#pragma warning disable CS0067 // raised once state transitions are implemented
        public event Action<GameState> OnStateChanged;
#pragma warning restore CS0067

        public GameState State { get; private set; } = GameState.Playing;
    }
}
