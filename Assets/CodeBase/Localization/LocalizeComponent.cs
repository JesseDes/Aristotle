using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizeComponent : MonoBehaviour
{
    [SerializeField]
    private string _localizationKey;

    void Start()
    {
        var textMeshProText = gameObject.GetComponent<TextMeshProUGUI>();

        if (textMeshProText != null)
        {
            textMeshProText.text = Controller.instance.LocalizationController.getLocalization(_localizationKey);
        }

        var unityText = gameObject.GetComponent<Text>();

        if (unityText != null)
        {
            unityText.text = Controller.instance.LocalizationController.getLocalization(_localizationKey);
        }

    }
}
