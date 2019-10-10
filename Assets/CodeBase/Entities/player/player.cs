using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    PlayerInputProfile inputProfile;
    Rigidbody playerRigidBody;
    // Start is called before the first frame update
    void Start()
    {
        inputProfile = new PlayerInputProfile();
        playerRigidBody = GetComponent<Rigidbody>();

        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveUp, moveUp);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveDown, moveDown);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveRight, moveRight);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.jump, jump);
    }

    // Update is called once per frame
    void Update()
    {
        inputProfile.checkInput();
    }

    void moveUp()
    {
        //Does nothing for now.
        //Will influence direction of fire dash and climbing up with earth.
    }

    void moveDown()
    {
        //Does nothing for now.
        //Will influence direction of fire dash and climbing down with earth.
    }

    void moveLeft()
    {
        transform.Translate(Vector2.left);
    }

    void moveRight()
    {
        transform.Translate(Vector2.right);
    }

    void jump()
    {
        playerRigidBody.AddForce(Vector3.up, ForceMode.Impulse);
    }
}
