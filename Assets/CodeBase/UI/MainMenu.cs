using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainMenu;

    [SerializeField]
    private GameObject _optionsMenu;

    [SerializeField]
    private GameObject _creditsMenu;

    public void Start()
    {
        UI_MainMenu();
    }

    public void UI_Start()
    {
        Controller.instance.Dispatch(EngineEvents.ENGINE_LOAD_START);
    }

    public void UI_Options()
    {
        _mainMenu.SetActive(false);
        _optionsMenu.SetActive(true);
        _creditsMenu.SetActive(false);
    }

    public void UI_MainMenu()
    {
        _mainMenu.SetActive(true);
        _optionsMenu.SetActive(false);
        _creditsMenu.SetActive(false);
    }

    public void UI_Credits()
    {
        _mainMenu.SetActive(false);
        _optionsMenu.SetActive(false);
        _creditsMenu.SetActive(true);
    }

    public void UI_Quit()
    {
        Application.Quit();
    }
}
