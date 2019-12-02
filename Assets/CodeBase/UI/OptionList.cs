using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionList : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        if (!PlayerPrefs.HasKey(SaveKeys.LEVEL))
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
