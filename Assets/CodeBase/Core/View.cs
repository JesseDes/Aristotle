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
    public GameObject testFab;


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
}
