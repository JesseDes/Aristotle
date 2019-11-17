using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Controller.instance.AddEventListener(EngineEvents.ENGINE_GAME_START, Activate);
    }

    private void Activate(System.Object response)
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
