using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI promptText;
    [SerializeField]
    public TextMeshProUGUI progressText;


    public void RestartLoader()
    {
        promptText.text = "Loading";
    }

    public void UpdateProgress(float progress)
    {
        progressText.text = (int)(progress * 100) + "%";

    }

    public void CompleteLoad()
    {
        progressText.text = "100%";
        promptText.text = "Press any key to continue";
    }
}
