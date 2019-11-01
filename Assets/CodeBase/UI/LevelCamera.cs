using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelCamera : MonoBehaviour
{
    public float cameraPanSpeed = 1;
    [HideInInspector]
    public AEvent panCompleteEvent { get; private set; }

    private bool _resetState;
    private Vector3 _originalPos;
    private string _eventKey;
    private float _lerpTimer;
    private void Awake()
    {
        _eventKey = Guid.NewGuid().ToString();
        panCompleteEvent = new AEvent(true, _eventKey);

    }

    // Start is called before the first frame update
    void Start()
    {
        
        Controller.instance.stateMachine.AddStateListener(onRespawn, EngineState.PLAYER_DEAD);

        //ALSO TEMP JUST FOR THE DEMO
        _originalPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_resetState)
        {
            _lerpTimer += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _originalPos, _lerpTimer / cameraPanSpeed);
            if(Camera.main.transform.position == _originalPos)
            {
                panCompleteEvent.Dispatch(null,_eventKey);
                _resetState = false;
                Model.instance.currentCheckpoint.StartSpawn();
            }
        }
    }

    private void onRespawn(System.Object repsonse)
    {
        _resetState = true;
        _lerpTimer = 0;
    }
}
