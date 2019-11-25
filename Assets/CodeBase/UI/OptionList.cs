using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionList : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey(SaveKeys.LEVEL))
        {
            transform.GetChild(1).gameObject.SetActive(false);
            for (int i = 2; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.position.Set(child.position.x, child.position.y + 70, child.position.z);
            }
        }
    }
}
