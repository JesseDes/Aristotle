using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class View : MonoBehaviour
{
    [HideInInspector]
    public static View instance;
    public int sceneCount = 0;
    [HideInInspector]
    public int currentLevel;
    public float sideScreenDetectionSize = 0.2f;
    public float topScreenDetectionSize = 0.2f;
    public float bottomScreenDetectionSize = 0.2f;
    public float horizontalCameraPanSpeed = 15;
    public float verticalCameraPanSpeed = 20;
    [HideInInspector]
    public bool mainMenuOpen { get => _mainMenu.isActiveAndEnabled; }
    [SerializeField]
    private AbilityOverlay _abilities = default;
    [SerializeField]
    private MainMenu _mainMenu = default;
    [SerializeField]
    private GameObject _pauseMenu = default;
    [SerializeField]
    private LoadingScreen _loadingScreen = default;

    private GameObject HUD;
    private Camera mainCamera;
    public GameObject player;
    public GameObject worldEdgePrefab;
    private bool _initFlag;
    private bool _firstLoad;
    private float _initFrameCounter;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            Screen.SetResolution(960, 720, true);
        }
        else if (instance != this)
            Destroy(gameObject);
    }
    
    public void Start()
    {
        Controller.instance.stateMachine.AddStateListener(OnStateChange);
        Controller.instance.AddEventListener(EngineEvents.ENGINE_GAME_INIT,addEdges);
        Controller.instance.AddEventListener(EngineEvents.ENGINE_CUTSCENE_END,(System.Object e) => NextLevel());
        mainCamera = Camera.main;
        player = GameObject.Find("Player");
        _loadingScreen.gameObject.SetActive(false);
        _firstLoad = true;

        ShowMainMenu();

        if (PlayerPrefs.HasKey(SaveKeys.LEVEL))
            GotoLevel(PlayerPrefs.GetInt(SaveKeys.LEVEL));
        else
            NextLevel();
    }

    public void Update()
    {
        if(_initFlag)           //When we load a level we need to give it atleast one frame to run all the starts/awakes of the gameobjects
        {
            _initFrameCounter++;
            if (_initFrameCounter <= 1)
            {
                _loadingScreen.gameObject.SetActive(false);
                Controller.instance.Dispatch(EngineEvents.ENGINE_LOAD_START);
                _initFlag = false;
                _initFrameCounter = 0;

            }
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            GotoLevel(1);
            PlayerPrefs.SetInt(SaveKeys.ACTIVE_ABILITIES, 1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            GotoLevel(2);
            PlayerPrefs.SetInt(SaveKeys.ACTIVE_ABILITIES, 2);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            GotoLevel(3);
            PlayerPrefs.SetInt(SaveKeys.ACTIVE_ABILITIES, 3);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            GotoLevel(4);
            PlayerPrefs.SetInt(SaveKeys.ACTIVE_ABILITIES, 4);
        }
    }

    public void addEdges(System.Object e)
    {
        if(worldEdgePrefab == null)
        {
            Debug.LogWarning("WARNING: You have no WorldEdges, the camera will not follow the player. \n Please Put a world Edge prefab in the View component");
            return;
        }

        float height = 2 * Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;
        
        //Right Edge
        GameObject worldEdge = Instantiate(worldEdgePrefab);
        worldEdge.GetComponent<WorldEdge>().Initialize(height, width * sideScreenDetectionSize, new Vector2((width / 2) - (width * sideScreenDetectionSize / 2), 0), -horizontalCameraPanSpeed);
       
        //Left Edge
        worldEdge = Instantiate(worldEdgePrefab);
        worldEdge.GetComponent<WorldEdge>().Initialize(height, width * sideScreenDetectionSize, new Vector2(-1 *  ((width / 2) - (width * sideScreenDetectionSize / 2)), 0), horizontalCameraPanSpeed);
        
        //Top Edge  
        worldEdge = Instantiate(worldEdgePrefab);
        worldEdge.GetComponent<WorldEdge>().Initialize(height * topScreenDetectionSize, width, new Vector2(0, ((height /2) ) - (height * topScreenDetectionSize / 2)), -verticalCameraPanSpeed, true);
        
        //Bottom Edge
        worldEdge = Instantiate(worldEdgePrefab);
        worldEdge.GetComponent<WorldEdge>().Initialize(height * bottomScreenDetectionSize, width, new Vector2(0, (-1 * ((height / 2)) + (height * bottomScreenDetectionSize / 2))), verticalCameraPanSpeed, true);
        
    }

    public void OnDestroy()
    {
        Controller.instance.stateMachine.RemoveStateListener(OnStateChange);
    }

    private void OnStateChange(System.Object response)
    {
        if (Controller.instance.stateMachine.state == EngineState.MENU)
        {
            ShowMainMenu();
        }
        else if (Controller.instance.stateMachine.state == EngineState.ACTIVE)
        {
            _abilities.gameObject.SetActive(true);
            _mainMenu.gameObject.SetActive(false);
        }
    }

    public void ShowMainMenu()
    {
        _abilities.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(true);
        Model.instance.audioManager.PlayBackgroundMusic(Model.instance.globalAudio.profileKey, "menu_music");
    }

    public void ShowPauseMenu()
    {
        _pauseMenu.gameObject.SetActive(true);
    }

    public void UpdateAbilitySymbol(ActiveAbility ability, AbilitySymbolState state)
    {
        _abilities.UpdateAbilitySymbol(ability, state);
    }

    private void NextLevel()
    {
        currentLevel++;
        StartCoroutine(LoadScene());
    }

    public void GotoLevel(int level)
    {
        currentLevel = level;
        StartCoroutine(LoadScene());
    }
    
    IEnumerator LoadScene()
    {
        string currentScene = "level" + currentLevel;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene);
        asyncLoad.allowSceneActivation = _firstLoad;
        _loadingScreen.RestartLoader();
        _loadingScreen.gameObject.SetActive(!_firstLoad);
        while (!asyncLoad.isDone)
        {
            _loadingScreen.UpdateProgress(asyncLoad.progress);

            if(asyncLoad.progress >= 0.9f && !_firstLoad)
            {
                _loadingScreen.CompleteLoad();

                if (Input.anyKeyDown)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        if (asyncLoad.isDone)
        {
            _initFlag = true;
            _firstLoad = false;
        }
    }
}
