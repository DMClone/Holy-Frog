using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButtonManager : MonoBehaviour
{
    [SerializeField] private ButtonCollection _buttonCollection;
    [SerializeField] private GameObject _levelButton;
    private void Start()
    {
        for (int i = 0; i < LevelManager.instance.levelsUnlocked; i++)
        {
            LevelButton levelButton = Instantiate(_levelButton, gameObject.transform).GetComponent<LevelButton>();
            levelButton.gameObject.name = "Button" + (i + 1);
            levelButton.level = i + 1;

            levelButton.gameObject.GetComponent<Button>().interactable = false;
            _buttonCollection.buttons.Add(levelButton.GetComponent<Button>());
            if (i == 0)
            {
                HomeMenu.instance.screens[1].firstButton = levelButton.gameObject.GetComponent<Button>();
            }
            levelButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1) + "";
        }
    }



    public void DisableAllButtons()
    {
        _buttonCollection.InteractableToggle();
    }
}
