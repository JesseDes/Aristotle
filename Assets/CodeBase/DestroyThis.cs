using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to destroy object it is attached to after a specified delay in seconds.
/// </summary>
public class DestroyThis : MonoBehaviour
{
    public float delay;
    
    // Start is called before the first frame update
    void Awake() {
        Destroy(this.gameObject, delay);
    }
}
