﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    [HideInInspector]
    public static Model instance;
    [HideInInspector]
    public CheckPoint currentCheckpoint { get; private set; }


    public AudioManager audioManager;
    public AudioProfile globalAudio;
    [HideInInspector]
    public AudioProfile currentLevelProfile;
    private void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            audioManager.init();
            audioManager.LoadProfile(globalAudio);
        }
        else if(instance != this)
        {
            Destroy(gameObject);

        }
    }
    
    // Start is called before the first frame update
    void Start()
    {


        Controller.instance.AddEventListener(EngineEvents.ENGINE_LOAD_START, LevelReady);
        Controller.instance.AddEventListener(EngineEvents.ENGINE_GAME_INIT, AudioStart);

        //Controller.instance.AddEventListener(EngineEvents.ENGINE_GAME_OVER, AudioStop);
    }

    private void OnDestroy()
    {
        audioManager.ClearProfiles(globalAudio.profileKey);
        audioManager.clearLevelAudio();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
            PlayerPrefs.DeleteKey(SaveKeys.CHECK_POINT);
        if (Input.GetKeyDown(KeyCode.H))
            PlayerPrefs.DeleteKey(SaveKeys.LEVEL);
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            PlayerPrefs.SetInt(SaveKeys.ACTIVE_ABILITIES, 4);
            PlayerPrefs.SetInt(SaveKeys.LEVEL, 4);
            PlayerPrefs.SetString(SaveKeys.CHECK_POINT, "8");
            View.instance.GotoLevel(4);
        }
    }

    public void SetCheckPoint(CheckPoint nextCheckpoint)
    {
        currentCheckpoint = nextCheckpoint;
        PlayerPrefs.SetString(SaveKeys.CHECK_POINT,currentCheckpoint.ID);
        PlayerPrefs.Save();
        Camera.main.GetComponent<LevelCamera>().setPanPosition(currentCheckpoint.CameraPanPosition);
    }

    private void LevelReady(System.Object e)
    {
        audioManager.clearLevelAudio();
        bool getStart = true;

        if (PlayerPrefs.GetInt(SaveKeys.LEVEL) == View.instance.currentLevel && PlayerPrefs.GetString(SaveKeys.CHECK_POINT) != "")
            getStart = false;

        PlayerPrefs.SetInt(SaveKeys.LEVEL, View.instance.currentLevel);
        
        foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("CheckPoint"))
        {
            if (!getStart && checkpoint.GetComponent<CheckPoint>().ID == PlayerPrefs.GetString(SaveKeys.CHECK_POINT))
                SetCheckPoint(checkpoint.GetComponent<CheckPoint>());
             if (getStart && checkpoint.GetComponent<CheckPoint>().startPoint)
                    SetCheckPoint(checkpoint.GetComponent<CheckPoint>());

        }
        
        if (currentCheckpoint)
            Camera.main.GetComponent<LevelCamera>().setPanPosition(currentCheckpoint.CameraPanPosition);
        else
            Debug.LogError("No starting spawn found, have you rememberd to set the flag in your checkpoint?");


        currentLevelProfile =  Resources.Load<AudioProfile>("level" + View.instance.currentLevel);
        audioManager.LoadProfile(currentLevelProfile);

        Controller.instance.Dispatch(EngineEvents.ENGINE_LOAD_FINISH);

        if (!View.instance.mainMenuOpen)
            Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_INIT);
    }
    
    private void AudioStart(System.Object e)
    {
        audioManager.PlayBackgroundMusic(currentLevelProfile.profileKey, "BGM");
    }

    public void ClearCheckPoint()
    {
        foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("CheckPoint"))
        {
            if (checkpoint.GetComponent<CheckPoint>().startPoint)
            {
                SetCheckPoint(checkpoint.GetComponent<CheckPoint>());
                break;
            }
        }
    }
}
