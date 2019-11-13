using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    [HideInInspector]
    public static Model instance;
    [HideInInspector]
    public CheckPoint currentCheckpoint { get; private set; }
    
    private void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //TEMP WILL FIX AFTER DEMO
        Controller.instance.AddEventListener(EngineEvents.ENGINE_LOAD_START, LevelReady);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayerPrefs.SetString(SaveKeys.CHECK_POINT, "");
        }
    }

    public void SetCheckPoint(CheckPoint nextCheckpoint)
    {
        currentCheckpoint = nextCheckpoint;
        PlayerPrefs.SetString(SaveKeys.CHECK_POINT,currentCheckpoint.ID);
        Camera.main.GetComponent<LevelCamera>().setPanPosition(currentCheckpoint.CameraPanPosition);

        /*GameObject.Find("Level Camera").transform.position = new Vector3(
            currentCheckpoint.CameraPanPosition.x,
            currentCheckpoint.CameraPanPosition.y,
            -66.0f);*/
    }

    private void LevelReady(System.Object response)
    {
        bool getStart = true;
        if (PlayerPrefs.GetInt(SaveKeys.LEVEL) == View.instance.currentLevel && PlayerPrefs.GetString(SaveKeys.CHECK_POINT) != "")
            getStart = false;

        foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("CheckPoint"))
        {
            if (!getStart && checkpoint.GetComponent<CheckPoint>().ID == PlayerPrefs.GetString(SaveKeys.CHECK_POINT))
                SetCheckPoint(checkpoint.GetComponent<CheckPoint>());
            else if (getStart && checkpoint.GetComponent<CheckPoint>().startPoint)
                    SetCheckPoint(checkpoint.GetComponent<CheckPoint>());

        }

        if (currentCheckpoint)
        {
            Camera.main.GetComponent<LevelCamera>().setPanPosition(currentCheckpoint.CameraPanPosition);
            Controller.instance.Dispatch(EngineEvents.ENGINE_LOAD_FINISH);
        }
        else
            Debug.LogError("No starting spawn found, have you rememberd to set the flag in your checkpoint?");
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }
}
