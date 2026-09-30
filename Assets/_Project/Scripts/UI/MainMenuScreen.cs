using AntLion.Core;
using UnityEngine;

namespace AntLion.UI
{
    // Step 6: Play / Instructions / Quit buttons. Display only; scene changes go through SceneLoader.
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private InstructionsPanel instructions;
        [SerializeField] private string arena = "Arena";

        public void Play()
        {
            sceneLoader.Load(arena);
        }

        public void ShowInstructions()
        {
        }

        public void Quit()
        {
            Application.Quit();
            Debug.Log("Game quit");
        }
    }
}
