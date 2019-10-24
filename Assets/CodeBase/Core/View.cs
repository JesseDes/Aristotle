using UnityEngine;
using UnityEngine.SceneManagement;

public class View : MonoBehaviour
{
    public static View instance;
    public int sceneCount = 0;

    [SerializeField]
    private AbilityOverlay _abilities = default;
    [SerializeField]
    private MainMenu _mainMenu = default;

    private GameObject HUD;
    private Camera mainCamera;
    public GameObject player;

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
    }

    public void Update()
    {
        ShiftCameraToPlayer();
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
