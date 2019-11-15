using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void UI_MainMenu()
    {
        Controller.instance.ResetGame();
        gameObject.SetActive(false);
    }

    public void UI_Continue()
    {
        Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_START);
        gameObject.SetActive(false);
    }
}
