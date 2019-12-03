using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DroneLookDirection {
    LEFT,
    RIGHT
};

public class DroneLook : MonoBehaviour
{
    //note: Start rotation is 90 degrees on the z axis
    public DroneLookDirection rotateDirection;

    public float minZRotation, maxZRotation;

    public float speed = 0.25f;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (transform.eulerAngles.z < minZRotation) {
            rotateDirection = DroneLookDirection.RIGHT;
        }
        if (transform.eulerAngles.z > maxZRotation) {
            rotateDirection = DroneLookDirection.LEFT;
        }

        if (rotateDirection == DroneLookDirection.RIGHT) {
            transform.Rotate(new Vector3(0, 0, speed));
        }
        else if (rotateDirection == DroneLookDirection.LEFT) {
            transform.Rotate(new Vector3(0, 0, -speed));
        }
    }
}
