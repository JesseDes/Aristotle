using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sphere : MonoBehaviour
{
    SphereInputProfile inputProfile;
    // Start is called before the first frame update
    void Start()
    {
        inputProfile = new SphereInputProfile();
        
        inputProfile.addListener(InputEvent.Key, SphereInputProfile.moveUp, moveUp);
        inputProfile.addListener(InputEvent.Key, SphereInputProfile.moveDown, moveDown);
        inputProfile.addListener(InputEvent.Key, SphereInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, SphereInputProfile.moveRight, moveRight);
    }

    // Update is called once per frame
    void Update()
    {
        inputProfile.checkInput();

    }

    private void moveUp()
    {
        transform.Translate(Vector3.up);
    }

    private void moveDown()
    { 
        transform.Translate(Vector3.down);
    }

    private void moveLeft()
    {
        transform.Translate(Vector2.left);
    }

    private void moveRight()
    {
        transform.Translate(Vector2.right); 
    }
}
