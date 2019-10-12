using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float moveSpeed;

    [SerializeField]
    float jumpSpeed;

    PlayerInputProfile inputProfile;
    Rigidbody2D playerRigidBody;

    private bool _isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        inputProfile = new PlayerInputProfile();
        playerRigidBody = GetComponent<Rigidbody2D>();

        _isGrounded = false;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Fuck");
        if (collision.gameObject.CompareTag("Floor"))
        {
            Debug.Log("You");
            //Setting the free-fall velocity to 0 prevents boosted jumps at corners.
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0.0f);
            _isGrounded = true;
        }
    }

    void moveUp()
    {
        //Does nothing for now.
        //Will influence direction of fire dash and climbing up with earth.
        jump(); //Unsure whether the up key or space is a better jump button.
    }

    void moveDown()
    {
        //Does nothing for now.
        //Will influence direction of fire dash and climbing down with earth.
    }

    void moveLeft()
    { 
        playerRigidBody.AddForce(Vector2.left * moveSpeed);
    }

    void moveRight()
    {
        playerRigidBody.AddForce(Vector2.right * moveSpeed);
    }

    void jump()
    {
        if (_isGrounded)
        {
            _isGrounded = false;
            playerRigidBody.AddForce(Vector2.up*jumpSpeed, ForceMode2D.Impulse);
        }
    }
}
