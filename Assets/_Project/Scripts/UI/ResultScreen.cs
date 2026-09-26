using AntLion.Core;
using UnityEngine;

namespace AntLion.UI
{
    // Step 6: win or lose screen with Retry and Menu buttons. One script serves both scenes.
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;

        public void Retry()
        {
        }

        public void BackToMenu()
        {
        }
    }
}
