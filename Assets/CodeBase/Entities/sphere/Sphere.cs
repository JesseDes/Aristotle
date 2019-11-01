using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sphere : MonoBehaviour
{
    SphereInputProfile inputProfile;
    Rigidbody2D rigidbody2D;
    Vector2 oldForce;

    private float movementForce = 10;
    private State<int,int> test;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        inputProfile = new SphereInputProfile();
        
        inputProfile.addListener(InputEvent.Key, SphereInputProfile.moveUp, moveUp);
        inputProfile.addListener(InputEvent.Key, SphereInputProfile.moveDown, moveDown);
        inputProfile.addListener(InputEvent.Key, SphereInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, SphereInputProfile.moveRight, moveRight);
        Controller.instance.stateMachine.AddStateListener(onStateChange);

    }

    private void testListener(System.Object response)
    {

        Debug.Log("hi!" + Convert.ToString(response));
    }

    private void onStateChange(System.Object response)
    {
        if (Controller.instance.stateMachine.state == EngineState.MENU)
        {
            this.enabled = false;
            oldForce = rigidbody2D.velocity;
            rigidbody2D.Sleep(); // important store velocity BEFORE sleeping

        }
        else if (Controller.instance.stateMachine.state == EngineState.ACTIVE)
        {
            this.enabled = true;
            rigidbody2D.WakeUp();
            rigidbody2D.velocity = oldForce; // apply velocity AFTER waking
        }
       
    }


    // Update is called once per frame
    void Update()
    {
        inputProfile.checkInput();

        /* Note in this example you will get 4 responses 2 spheres are listening, and 2 sphere dispatch */
        if (Input.GetKeyUp(KeyCode.X))
            Controller.instance.Dispatch(EngineEvents.ENGINE_CHECKPOINT_REACHED);
        if(Input.GetKeyUp(KeyCode.Space))
            Controller.instance.AddEventListener(EngineEvents.ENGINE_CHECKPOINT_REACHED, testListener);

    }

    private void moveUp()
    {
        rigidbody2D.AddForce(Vector2.up * movementForce);
        
    }

    private void moveDown()
    { 
        rigidbody2D.AddForce(Vector2.down * movementForce);
        Controller.instance.RemoveEventListener(EngineEvents.ENGINE_CHECKPOINT_REACHED, testListener);
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
