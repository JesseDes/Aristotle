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

    [SerializeField]
    float climbSpeed;

    private const float NORMAL_MOVEMENT_SPEED = 5.0f;
    private const float NORMAL_JUMP_SPEED = 7.5f;
    private const float NORMAL_MASS = 1.0f;

    private const float WIND_JUMP_SPEED = NORMAL_JUMP_SPEED * 1.25f;
    private const float WIND_MASS = NORMAL_MASS * 0.8f;

    private const float EARTH_CLIMB_SPEED = 3.0f;

    ActiveAbility currentAbility;

    PlayerInputProfile inputProfile;
    Rigidbody2D playerRigidBody;

    private bool _isGrounded;
    private bool _isHuggingWall;
    private bool _isClimbing;
    private Vector3 _storedForce;
    // Start is called before the first frame update
    void Start()
    {
        inputProfile = new PlayerInputProfile();
        playerRigidBody = GetComponent<Rigidbody2D>();

        _isGrounded = false;
        _isHuggingWall = false;
        _isClimbing = false;

        moveSpeed = NORMAL_MOVEMENT_SPEED;
        jumpSpeed = NORMAL_JUMP_SPEED;
        climbSpeed = EARTH_CLIMB_SPEED;
        playerRigidBody.mass = NORMAL_MASS;

        //Listeners for movement and jumping.
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveRight, moveRight);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveLeft, stopMoving);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveRight, stopMoving);

        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveUp, moveUp);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveDown, moveDown);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveUp, stopClimbing);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveDown, stopClimbing);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.jump, jump);

        //Listeners for abilities.
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleIce, toggleIce);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleFire, toggleFire);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleWind, toggleWind);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleEarth, toggleEarth);

        currentAbility = ActiveAbility.NORMAL;
        this.enabled = false;
        Controller.instance.stateMachine.AddStateListener(onStateChange);
    }

    private void onStateChange(System.Object response)
    {
        if (Controller.instance.stateMachine.state == EngineState.MENU)
        {
            this.enabled = false;
            _storedForce = playerRigidBody.velocity;
            playerRigidBody.Sleep(); 

        }
        else if (Controller.instance.stateMachine.state == EngineState.ACTIVE)
        {
            this.enabled = true;
            playerRigidBody.WakeUp();
            playerRigidBody.velocity = _storedForce; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        inputProfile.checkInput();
        if (_isHuggingWall && !_isClimbing && currentAbility.Equals(ActiveAbility.EARTH))
        {
            setYVelocity(0.0f);
        }
    }

    public void OnDestroy()
    {
        Controller.instance.stateMachine.RemoveStateListener(onStateChange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            //Setting the free-fall velocity to 0 prevents boosted jumps at corners.
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0.0f);
            _isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            _isHuggingWall = true;
            if (currentAbility.Equals(ActiveAbility.EARTH))
            {
                startHuggingWall();
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0.0f);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            stopHuggingWall();
        }
    }

    void moveUp()
    {
        if (_isHuggingWall && currentAbility.Equals(ActiveAbility.EARTH))
        {
            _isClimbing = true;
            setYVelocity(climbSpeed);
        }
    }

    void moveDown()
    {
        if (_isHuggingWall && currentAbility.Equals(ActiveAbility.EARTH))
        {
            _isClimbing = true;
            setYVelocity(-climbSpeed);
        }
    }

    void moveLeft()
    {
        setXVelocity(-moveSpeed);
    }

    void moveRight()
    {
        setXVelocity(moveSpeed);
    }

    void stopMoving()
    {
        setXVelocity(0.0f);
    }

    void stopClimbing()
    {
        _isClimbing = false;
        if (_isHuggingWall)
        {
            setYVelocity(0.0f);
        }
    }

    void startHuggingWall()
    {
        _isHuggingWall = true;
        playerRigidBody.gravityScale = 0;
    }
    void stopHuggingWall()
    {
        _isHuggingWall = false;
        _isClimbing = false;
        playerRigidBody.gravityScale = 1;
    }

    void jump()
    {
        if (_isGrounded) //TODO: Add statement to prevent jumping while ice is active.
        {
            _isGrounded = false;
            playerRigidBody.AddForce(Vector2.up*jumpSpeed, ForceMode2D.Impulse);
        }
        else if (_isHuggingWall && currentAbility.Equals(ActiveAbility.EARTH))
        {
            stopHuggingWall();
            playerRigidBody.AddForce(Vector2.up*jumpSpeed, ForceMode2D.Impulse);
        }
    }

    void setXVelocity(float newXVelocity)
    {
        playerRigidBody.velocity = new Vector2(newXVelocity, playerRigidBody.velocity.y);
    }

    void setYVelocity(float newYVelocity)
    {
        playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, newYVelocity);
    }

    //NOTE: Changing the sprite color is a temporary measure until proper animations are
    //implemented.
    void toggleIce()
    {
        if (currentAbility.Equals(ActiveAbility.EARTH))
        {
            stopHuggingWall();
        }
        if (!currentAbility.Equals(ActiveAbility.ICE))
        {
            currentAbility = ActiveAbility.ICE;
            //May need to add ice constants for these properties.
            jumpSpeed = NORMAL_JUMP_SPEED;
            playerRigidBody.mass = NORMAL_MASS;
        }
        else
        {
            currentAbility = ActiveAbility.NORMAL;
            GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        //TBD this iteration.
    }

    void toggleFire()
    {
        if (currentAbility.Equals(ActiveAbility.EARTH))
        {
            stopHuggingWall();
        }
        if (!currentAbility.Equals(ActiveAbility.FIRE))
        {
            currentAbility = ActiveAbility.FIRE;
            GetComponent<SpriteRenderer>().color = Color.red;
            jumpSpeed = NORMAL_JUMP_SPEED;
            playerRigidBody.mass = NORMAL_MASS;
        }
        else
        {
            deactivateAbility();
        }
        //TBD next iteration.
    }

    void toggleWind()
    {
        if (currentAbility.Equals(ActiveAbility.EARTH))
        {
            stopHuggingWall();
        }
        if (!currentAbility.Equals(ActiveAbility.WIND))
        {
            currentAbility = ActiveAbility.WIND;
            GetComponent<SpriteRenderer>().color = Color.green;
            jumpSpeed = WIND_JUMP_SPEED;
            playerRigidBody.mass = WIND_MASS;
        }
        else
        {
            deactivateAbility();
        }
    }

    void toggleEarth()
    {
        if (currentAbility.Equals(ActiveAbility.EARTH))
        {
            stopHuggingWall();
        }
        if (!currentAbility.Equals(ActiveAbility.EARTH))
        {
            currentAbility = ActiveAbility.EARTH;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            jumpSpeed = NORMAL_JUMP_SPEED;
            playerRigidBody.mass = NORMAL_MASS;
            if (_isHuggingWall)
            {
                startHuggingWall();
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0.0f);
            }
        }
        else
        {
            deactivateAbility();
        }
        //TBD next iteration.
    }

    void deactivateAbility()
    {
        moveSpeed = NORMAL_MOVEMENT_SPEED; //In anticipation for ice slowing movement down.
        jumpSpeed = NORMAL_JUMP_SPEED;
        playerRigidBody.mass = NORMAL_MASS;
        currentAbility = ActiveAbility.NORMAL;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
