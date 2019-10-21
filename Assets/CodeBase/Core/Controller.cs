using System.Collections.Generic;
using UnityEngine;
using System;


public class Controller : MonoBehaviour
{
    /// <summary>
    /// The Controller is Responsible for Event Dispatching for Application wide Events and States
    /// </summary>
    [HideInInspector]
    public static Controller instance;
    [HideInInspector]
    public LocalizationController LocalizationController;
    [HideInInspector]
    public ControllerStateMachine stateMachine { get; private set; }


    [SerializeField]
    private LocalizationData _localizationData;
    [SerializeField]
    private string _defaultLanguageCode = "en";

    private Dictionary<EngineEvents, AEvent> _eventList;


    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LocalizationController = new LocalizationController(_localizationData);
        LocalizationController.setLanguage(_defaultLanguageCode);

        _eventList = new Dictionary<EngineEvents, AEvent>();
        _eventList.Add(EngineEvents.ENGINE_CHECKPOINT_REACHED, new AEvent());
        _eventList.Add(EngineEvents.ENGINE_GAME_OVER, new AEvent());
        _eventList.Add(EngineEvents.ENGINE_GAME_PAUSE, new AEvent());
        _eventList.Add(EngineEvents.ENGINE_GAME_START, new AEvent());
        _eventList.Add(EngineEvents.ENGINE_LOAD_LEVEL, new AEvent());
        _eventList.Add(EngineEvents.ENGINE_STAGE_COMPLETE, new AEvent());
        stateMachine = new ControllerStateMachine(); // Must be created After events

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_START);
    }
    /// <summary>
    /// Add a function that will be called whenever an event is dispatched
    /// GARBAGE COLLECTION WILL NOT REMOVE EVENTS, YOU MUST DO THIS BEFORE OBJECT DESTRUCTION OR THERE WILL BE MEMORY LEAKS
    /// </summary>
    /// <param name="type"> The event you want to listen for</param>
    /// <param name="callback">The Function that you wish to recieve the response(MUST TAKE IN A System.Object param) </param>
    public void AddEventListener(EngineEvents type, Action<System.Object> callback)
    {
        if (_eventList.ContainsKey(type))
            _eventList[type].AddListener(callback);

    }

    /// <summary>
    /// Remove a function that was being called whenever an event is dispatched
    /// </summary>
    /// <param name="type"> The event you were listening for</param>
    /// <param name="callback">The Function that was recieving the response(MUST TAKE IN A System.Object param) </param>
    public void RemoveEventListener(EngineEvents type, Action<System.Object> callback)
    {
        if (_eventList.ContainsKey(type))
            _eventList[type].RemoveListener(callback);
    }

    /// <summary>
    /// Dispatch an EngineEvent.
    /// </summary>
    /// <param name="type"> The event you wish to disatch</param>
    public void Dispatch(EngineEvents type)
    {
        if (_eventList.ContainsKey(type))
            _eventList[type].Dispatch();
        else
            Debug.Log("Warning: No currenlty registered listners for event :" + type);
    }
}
