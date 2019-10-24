using UnityEngine;
using UnityEngine.SceneManagement;

public class View : MonoBehaviour
{
    [HideInInspector]
    public static View instance;
    public int sceneCount = 0;
    public float sideScreenDetectionSize = 0.2f;
    public float topScreenDetectionSize = 0.2f;
    public float bottomScreenDetectionSize = 0.2f;

    [SerializeField]
    private AbilityOverlay _abilities = default;
    [SerializeField]
    private MainMenu _mainMenu = default;

    private GameObject HUD;
    private Camera mainCamera;
    public GameObject player;
    public GameObject worldEdgePrefab;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;    
        }
        else if (instance != this)
            Destroy(gameObject);
    }
    
    public void nextScene()
    {
        sceneCount++;
        SceneManager.LoadScene(sceneCount);
    }

    public void Start()
    {
        Controller.instance.stateMachine.AddStateListener(OnStateChange);
        mainCamera = Camera.main;
        player = GameObject.Find("Player");
        addEdges();
    }

    public void Update()
    {
        ShiftCameraToPlayer();
    }

    private void addEdges()
    {
        float height = 2 * Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;
        
        //Right Edge
        GameObject worldEdge = Instantiate(worldEdgePrefab);
        worldEdge.GetComponent<WorldEdge>().Initialize(height, width * sideScreenDetectionSize, new Vector2((width / 2) - (width * sideScreenDetectionSize / 2), 0), -1);
       
        //Left Edge
        worldEdge = Instantiate(worldEdgePrefab);
        worldEdge.GetComponent<WorldEdge>().Initialize(height, width * sideScreenDetectionSize, new Vector2(-1 *  ((width / 2) - (width * sideScreenDetectionSize / 2)), 0), 1);
        
        //Top Edge  
        worldEdge = Instantiate(worldEdgePrefab);
        worldEdge.GetComponent<WorldEdge>().Initialize(height * topScreenDetectionSize, width, new Vector2(0, ((height /2) ) - (height * topScreenDetectionSize / 2)), -1, true);
        
        //Bottom Edge
        worldEdge = Instantiate(worldEdgePrefab);
        worldEdge.GetComponent<WorldEdge>().Initialize(height * bottomScreenDetectionSize, width, new Vector2(0, (-1 * ((height / 2)) + (height * bottomScreenDetectionSize / 2))), 1, true);
        
    }

    public void OnDestroy()
    {
        Controller.instance.stateMachine.RemoveStateListener(OnStateChange);
    }

    private void OnStateChange(System.Object response)
    {
        if (Controller.instance.stateMachine.state == EngineState.MENU)
        {
            _abilities.gameObject.SetActive(false);
            _mainMenu.gameObject.SetActive(true);

        }
        else if (Controller.instance.stateMachine.state == EngineState.ACTIVE)
        {
            _abilities.gameObject.SetActive(true);
            _mainMenu.gameObject.SetActive(false);
        }

    }

    public void UpdateAbilitySymbol(ActiveAbility ability, AbilitySymbolState state)
    {
        _abilities.UpdateAbilitySymbol(ability, state);
    }

    void ShiftCameraToPlayer()
    {
        Vector3 playerPosition = player.GetComponent<Player>().transform.position;
        Vector3 cameraPosition = mainCamera.GetComponent<Camera>().transform.position;
        if (playerPosition.x - cameraPosition.x > mainCamera.aspect * mainCamera.orthographicSize * 0.6)
        {
            mainCamera.transform.Translate(Vector2.right * Time.deltaTime * 5.0f);
        }
        if (playerPosition.x - cameraPosition.x < mainCamera.aspect * mainCamera.orthographicSize * -0.6)
        {
            mainCamera.transform.Translate(Vector2.left * Time.deltaTime * 5.0f);
        }
        if (playerPosition.y - cameraPosition.y > mainCamera.orthographicSize * 0.6)
        {
            mainCamera.transform.Translate(Vector2.up * Time.deltaTime * 5.0f);
        }
        if (playerPosition.y - cameraPosition.y < mainCamera.orthographicSize * -0.6)
        {
            mainCamera.transform.Translate(Vector2.down * Time.deltaTime * 5.0f);
        }
    }
}
