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
        PlayerPrefs.SetString(SaveKeys.CHECK_POINT, "");
        PlayerPrefs.SetInt(SaveKeys.ACTIVE_ABILITIES, 1);
        Model.instance.audioManager.StopBackgroundMusic();
        if (PlayerPrefs.GetInt(SaveKeys.LEVEL) != 1)
        {

            Controller.instance.AddEventListener(EngineEvents.ENGINE_LOAD_FINISH, NewGameReady);
            View.instance.GotoLevel(1);
            gameObject.SetActive(false);
        }
        else
        {
            Model.instance.ClearCheckPoint();
            Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_INIT);
            gameObject.SetActive(false);
        }
    }

    private void NewGameReady(System.Object e)
    {
        if (gameObject.activeSelf)
        {
            Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_INIT);
            gameObject.SetActive(false);

        }
    }

    public void UI_Continue()
    {
        Model.instance.audioManager.StopBackgroundMusic();
        Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_INIT);
        gameObject.SetActive(false);

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
