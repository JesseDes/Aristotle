using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [HideInInspector]
    public static Controller instance;
    [HideInInspector]
    public LocalizationController LocalizationController;

    [SerializeField]
    private LocalizationData _localizationData;
    [SerializeField]
    private string _defaultLanguageCode = "en";


    private void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LocalizationController = new LocalizationController(_localizationData);
        LocalizationController.setLanguage(_defaultLanguageCode);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
            Model.instance.state++;

        if (Input.GetKey(KeyCode.N))
            View.instance.nextScene();

        if (Input.GetKey(KeyCode.D))
            Debug.Log("current state "  + Model.instance.state);
    }
}
