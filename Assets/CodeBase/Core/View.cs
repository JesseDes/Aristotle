using UnityEngine;
using UnityEngine.SceneManagement;

public class View : MonoBehaviour
{
    public static View instance;
    public int sceneCount = 0;

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
        Debug.Log(mainCamera.transform.position.x);
        addCanvasToMainCamera(testFab.GetComponent<Canvas>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
