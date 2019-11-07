using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDEnable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        this.GetComponent<Canvas>().enabled = true;
    }

}
