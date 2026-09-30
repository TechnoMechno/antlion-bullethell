using UnityEngine;
using UnityEngine.SceneManagement;

namespace AntLion.Core
{
    // Step 6: loads scenes by name, so GameStateManager and menu buttons never hardcode scene loading.
    public class SceneLoader : MonoBehaviour
    {
        public void Load(string sceneName)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }

        [ContextMenu("load menu")]
        public void testLoadMenu()
        {
            Load("MainMenu");
        }
    }
}
