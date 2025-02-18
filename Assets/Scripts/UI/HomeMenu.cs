using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenu : MonoBehaviour
{
    [SerializeField]
    private Screens[] screens;

    [System.Serializable]
    public class Screens
    {
        public GameObject rectTransform;
        public Button firstButton;
    }

    public void LoadStartScreen()
    {

    }

    public void LoadLevelScreen()
    {

    }


}


