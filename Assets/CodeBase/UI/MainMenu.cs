using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void UI_Start()
    {
        gameObject.SetActive(false);
        Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_START);
    }

    public void UI_Options()
    {
        //CLose menu 
        //Open options
    }
}
