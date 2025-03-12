using UnityEngine;

public class HomeMenuButton : MonoBehaviour
{
    [SerializeField] private int partOfScreen;
    [SerializeField] private int showScreen;

    public void MenuButton()
    {
        HomeMenu.instance.LoadScreen(partOfScreen, showScreen);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
