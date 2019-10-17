using UnityEngine;
using UnityEngine.SceneManagement;

public class View : MonoBehaviour
{
    public static View instance;
    public int sceneCount = 0;

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

    public void addCanvasToMainCamera(Canvas cameraCanvas)
    {
        Instantiate(cameraCanvas);
        cameraCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        cameraCanvas.worldCamera = mainCamera;

        //cameraCanvas;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.Find("Player");
        Debug.Log(mainCamera.transform.position.x);
        //addCanvasToMainCamera(testFab.GetComponent<Canvas>());
    }

    // Update is called once per frame
    void Update()
    {
        ShiftCameraToPlayer();
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
