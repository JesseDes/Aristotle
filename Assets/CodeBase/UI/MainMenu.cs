using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainMenu;

    [SerializeField]
    private GameObject _optionsMenu;

    public void Start()
    {
        UI_MainMenu();
    }

    public void UI_Start()
    {
        Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_START);
    }

    public void UI_Options()
    {
        _mainMenu.SetActive(false);
        _optionsMenu.SetActive(true);
    }

    public void UI_MainMenu()
    {
        _mainMenu.SetActive(true);
        _optionsMenu.SetActive(false);
    }
}
