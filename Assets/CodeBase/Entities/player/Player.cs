using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActiveAbility
{
    NORMAL,
    ICE,
    FIRE,
    WIND,
    EARTH
};

public class Player : MonoBehaviour
{
    [SerializeField]
    float moveSpeed;

    [SerializeField]
    float jumpSpeed;

    ActiveAbility currentAbility;

    PlayerInputProfile inputProfile;
    Rigidbody2D playerRigidBody;

    private bool _isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        inputProfile = new PlayerInputProfile();
        playerRigidBody = GetComponent<Rigidbody2D>();

        _isGrounded = false;

        //Listeners for movement and jumping.
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveUp, moveUp);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveDown, moveDown);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveRight, moveRight);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.jump, jump);

        //Listeners for abilities.
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleIce, toggleIce);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleFire, toggleFire);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleWind, toggleWind);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleEarth, toggleEarth);

        currentAbility = ActiveAbility.NORMAL;
    }

    // Update is called once per frame
    void Update()
    {
        inputProfile.checkInput();
        Debug.Log(currentAbility.ToString());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
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

    void toggleIce()
    {
        if (!currentAbility.Equals(ActiveAbility.ICE))
        {
            currentAbility = ActiveAbility.ICE;
        }
        else
        {
            currentAbility = ActiveAbility.NORMAL;
        }
        //TBD this iteration.
    }

    void toggleFire()
    {
        if (!currentAbility.Equals(ActiveAbility.FIRE))
        {
            currentAbility = ActiveAbility.FIRE;
        }
        else
        {
            deactivateAbility();
        }
        //TBD next iteration.
    }

    void toggleWind()
    {
        if (!currentAbility.Equals(ActiveAbility.WIND))
        {
            currentAbility = ActiveAbility.WIND;
        }
        else
        {
            deactivateAbility();
        }
    }

    void toggleEarth()
    {
        if (!currentAbility.Equals(ActiveAbility.EARTH))
        {
            currentAbility = ActiveAbility.EARTH;
        }
        else
        {
            deactivateAbility();
        }
        //TBD next iteration.
    }

    void deactivateAbility()
    {
        currentAbility = ActiveAbility.NORMAL;
    }
}
