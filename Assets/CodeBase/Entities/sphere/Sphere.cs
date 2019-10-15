using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sphere : MonoBehaviour
{
    SphereInputProfile inputProfile;
    Rigidbody2D rigidbody2D;

    private float movementForce = 10;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
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
        rigidbody2D.AddForce(Vector2.up * movementForce);
    }

    private void moveDown()
    { 
        rigidbody2D.AddForce(Vector2.down * movementForce);
    }

    private void moveLeft()
    {
        rigidbody2D.AddForce(Vector2.left * movementForce);
    }

    private void moveRight()
    {
        rigidbody2D.AddForce(Vector2.right * movementForce);
    }
}
