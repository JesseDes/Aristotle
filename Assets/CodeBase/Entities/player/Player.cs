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

    private const float NORMAL_MOVEMENT_SPEED = 5.0f;
    private const float NORMAL_JUMP_SPEED = 7.5f;
    private const float NORMAL_MASS = 1.0f;

    private const float WIND_JUMP_SPEED = NORMAL_JUMP_SPEED * 1.25f;
    private const float WIND_MASS = NORMAL_MASS * 0.8f;

    ActiveAbility currentAbility;

    PlayerInputProfile inputProfile;
    Rigidbody2D playerRigidBody;
    Animator animator;
    SpriteRenderer playerSpriteRenderer;

    private bool _isGrounded;
    private bool _isFalling;
    private float _currentSpeed = 0;
    private Vector3 _storedForce;

    // Start is called before the first frame update
    void Start()
    {
        inputProfile = new PlayerInputProfile();
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        moveSpeed = NORMAL_MOVEMENT_SPEED;
        jumpSpeed = NORMAL_JUMP_SPEED;
        playerRigidBody.mass = NORMAL_MASS;

        //Listeners for vertical movement.
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveRight, moveRight);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveLeft, stopMoving);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveRight, stopMoving);

        //Listeners for movement and jumping.
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveUp, moveUp);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveDown, moveDown);
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

    // Update is called once per frame
    void Update()
    {
        inputProfile.checkInput();

        // Animate player movement
        _currentSpeed = Mathf.Abs(Input.GetAxis("Horizontal") * moveSpeed);
        animator.SetFloat("speed", _currentSpeed);
        animator.SetBool("isGrounded", _isGrounded);
        animator.SetBool("isFalling", _isFalling);
    }

    private void FixedUpdate()
    {
        inputProfile.checkInput();

        // player is jumping
        if (playerRigidBody.velocity.y > 0.1)
        {
            _isGrounded = false;
        }

        // player is falling
        if (playerRigidBody.velocity.y < -0.1)
        {
            _isFalling = true;
            _isGrounded = false;
        }
        else
        {
            _isFalling = false;
        }
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
        setXVelocity(-moveSpeed);

        if(!playerSpriteRenderer.flipX)
        {
            playerSpriteRenderer.flipX = true;
        }
    }

    void moveRight()
    {
        setXVelocity(moveSpeed);

        if (playerSpriteRenderer.flipX)
        {
            playerSpriteRenderer.flipX = false;
        }
    }

    void stopMoving()
    {
        setXVelocity(0.0f);
    }

    void jump()
    {
        if (_isGrounded) //TODO: Add statement to prevent jumping while ice is active.
        {
            playerRigidBody.AddForce(Vector2.up*jumpSpeed, ForceMode2D.Impulse);
        }
    }

    void setXVelocity(float newXVelocity)
    {
        playerRigidBody.velocity = new Vector2(newXVelocity, playerRigidBody.velocity.y);
    }

    //NOTE: Changing the sprite color is a temporary measure until proper animations are
    //implemented.
    void toggleIce()
    {
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
        if (!currentAbility.Equals(ActiveAbility.EARTH))
        {
            currentAbility = ActiveAbility.EARTH;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            jumpSpeed = NORMAL_JUMP_SPEED;
            playerRigidBody.mass = NORMAL_MASS;
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
