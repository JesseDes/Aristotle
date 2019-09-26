using UnityEngine;
using UnityEngine.SceneManagement;

public class View : MonoBehaviour
{
    public static View instance;
    public int sceneCount = 0;

    private GameObject HUD;
    private Camera mainCamera;


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

    public void addToHud(GameObject HUDElement)
    {
        HUDElement.transform.SetParent(HUD.transform);
        Instantiate(HUDElement);
    }

    // Start is called before the first frame update
    void Start()
    {
        HUD = new GameObject();
        HUD.name = "HUD";
        HUD.AddComponent<Canvas>();
        Instantiate(HUD);

        HUD.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        HUD.GetComponent<Canvas>().worldCamera = Camera.main;

        DontDestroyOnLoad(HUD);

        //addToHud()
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
