using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelCamera : MonoBehaviour
{
    public float cameraPanSpeed = 1;
    [HideInInspector]
    public AEvent panStartEvent { get; private set; }
    public AEvent panCompleteEvent { get; private set; }

    private bool _resetState;
    private Vector3 _originalPos;
    private string _eventKey;
    private float _lerpTimer;
    private bool _fullPanState;
    private Vector3 _fullPanDirection;

    public void setPanPosition(Vector2 position)
    {
        _originalPos.x = position.x;
        _originalPos.y = position.y;
    }


    private void Awake()
    {
        _eventKey = Guid.NewGuid().ToString();
        panCompleteEvent = new AEvent(true, _eventKey);
        panStartEvent = new AEvent(true, _eventKey);
    }

    // Start is called before the first frame update
    void Start()
    {

        Controller.instance.AddEventListener(EngineEvents.ENGINE_LOAD_FINISH, onRespawn);
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
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _originalPos, Math.Min(_lerpTimer, cameraPanSpeed) / cameraPanSpeed);
            if (Camera.main.transform.position == _originalPos)
            {
                panCompleteEvent.Dispatch(null, _eventKey);
                _resetState = false;
                Model.instance.currentCheckpoint.StartSpawn();
                _lerpTimer = 0;

            }
        }
        else if(_fullPanState)
        {
            _lerpTimer += Time.deltaTime;
            
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _fullPanDirection, Math.Min(_lerpTimer,cameraPanSpeed) / cameraPanSpeed);
            if (Camera.main.transform.position == _fullPanDirection)
            {
                panCompleteEvent.Dispatch(null, _eventKey);
                _fullPanState = false;
                _lerpTimer = 0;
            }
        }

    }

    private void onRespawn(System.Object repsonse)
    {
        _resetState = true;
        panStartEvent.Dispatch(null,_eventKey);
        
    }

    public void FullScreenPan(Vector2 direction)
    {
        _fullPanState = true;
        _fullPanDirection = direction;
        float height = 2 * Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;

        direction.x = direction.x * width;
        direction.y = direction.y * height;

        _fullPanDirection = new Vector3(direction.x + transform.position.x, direction.y + transform.position.y, transform.position.z);
        panStartEvent.Dispatch(null, _eventKey);
    }
}
