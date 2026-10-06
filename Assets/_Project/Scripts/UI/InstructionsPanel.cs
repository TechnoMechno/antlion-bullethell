using UnityEngine;

namespace AntLion.UI
{
    // Step 6: the how-to-play panel. Opened by MainMenuScreen, closed by its X button.
    public class InstructionsPanel : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
