using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    float iceMagnitude;

    [SerializeField]
    float fireDashSpeed;

    [SerializeField]
    float climbSpeed;

    [HideInInspector]
    public ActiveAbility currentAbility { get; private set; }

    //Various parameters for each individual ability. If there is no counterpart for a specific ability, then
    //the parameter stays the same between the normal ability and the specified ability.
    private const float NORMAL_MOVEMENT_SPEED = 5.0f;
    private const float NORMAL_JUMP_SPEED = 7.5f;
    private const float NORMAL_MASS = 1.0f;

    private const float ICE_MOVEMENT_SPEED = NORMAL_MOVEMENT_SPEED * 0.5f;
    private const float ICE_FALL_MAGNITUDE = 15.0f;

    private const float FIRE_DASH_FORCE = 8.0f;
    private const float FIRE_DASH_DURATION = 0.5f; //Duration between start of dash and player falling again.

    private const float WIND_JUMP_SPEED = NORMAL_JUMP_SPEED * 1.1f;
    private const float WIND_MASS = NORMAL_MASS * 0.8f;

    private const float EARTH_CLIMB_SPEED = 4.0f;

    //Various properties of the player entity.
    PlayerInputProfile inputProfile;
    Rigidbody2D playerRigidBody;
    Animator animator;
    SpriteRenderer playerSpriteRenderer;

    private int horizontalDashDirection = 0;
    private int verticalDashDirection = 0;

    private bool _isGrounded;
    private bool _shiftPressed, _canDash, _dashing;
    private bool _isHuggingWall;
    private bool _isClimbing;
    private bool _isFalling;
    private float _currentSpeed = 0;
    private Vector3 _storedForce;
    private bool _isRespawn = false;

    public void init()
    {
        _isRespawn = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputProfile = new PlayerInputProfile();
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        _isGrounded = false;

        _canDash = true;
        _dashing = false;

        _isHuggingWall = false;
        _isClimbing = false;

        moveSpeed = NORMAL_MOVEMENT_SPEED;
        jumpSpeed = NORMAL_JUMP_SPEED;
        climbSpeed = EARTH_CLIMB_SPEED;
        playerRigidBody.mass = NORMAL_MASS;

        //Listeners for vertical movement.
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveRight, moveRight);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveLeft, stopMoving);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveRight, stopMoving);

        //Listeners for movement and jumping.
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveUp, moveUp);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveDown, moveDown);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveUp, stopClimbing);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveDown, stopClimbing);
        inputProfile.addListener(InputEvent.Down, PlayerInputProfile.jump, jump);

        //Listeners for abilities.
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleIce, toggleIce);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleFire, toggleFire);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleWind, toggleWind);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleEarth, toggleEarth);

        currentAbility = ActiveAbility.NORMAL;
        this.enabled = _isRespawn;
        Controller.instance.stateMachine.AddStateListener(onStateChange);

    }

    // Update is called once per frame
    private void Update()
    {
        inputProfile.checkInput();
    }

    void FixedUpdate()
    {
        //fire...
        if (currentAbility.Equals(ActiveAbility.FIRE)) {
        }

        //Player is on wall but choosing not to climb.
        if (_isHuggingWall && !_isClimbing && currentAbility.Equals(ActiveAbility.EARTH)) {
            setYVelocity(0.0f);
        }

        // player is jumping
        if (playerRigidBody.velocity.y >= 0.1)
        {
            _isGrounded = false;
            _isFalling = false;
        }
        // player is falling
        else if (playerRigidBody.velocity.y < -0.1)
        {
            _isGrounded = false;
            _isFalling = true;
        }

        // Animate player movement
        _currentSpeed = Mathf.Abs(Input.GetAxis("Horizontal") * moveSpeed);
        animator.SetFloat("speed", _currentSpeed);
        animator.SetBool("isGrounded", _isGrounded);
        animator.SetBool("isFalling", _isFalling);
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
            _canDash = true;
            _isFalling = false;
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
        if (collision.gameObject.CompareTag("Floor"))
        {
            _isGrounded = false;
            if (currentAbility.Equals(ActiveAbility.ICE))
            {
                //have player fall very fast if airborne when going off edge while ice is active.
                playerRigidBody.AddForce(Vector2.down * iceMagnitude, ForceMode2D.Impulse);
            }
        }

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
        else if (currentAbility.Equals(ActiveAbility.FIRE))
        {
            verticalDashDirection = 1;
        }
    }

    void moveDown()
    {
        if (_isHuggingWall && currentAbility.Equals(ActiveAbility.EARTH))
        {
            _isClimbing = true;
            setYVelocity(-climbSpeed);
        }
        else if (currentAbility.Equals(ActiveAbility.FIRE))
        {
            verticalDashDirection = -1;
        }
    }

    void moveLeft()
    {
        if (!_dashing) {
            setXVelocity(-moveSpeed);
            if (!playerSpriteRenderer.flipX) {
                playerSpriteRenderer.flipX = true;
            }
        }
        if (currentAbility.Equals(ActiveAbility.FIRE))
        {
            horizontalDashDirection = -1;
        }
    }

    void moveRight()
    {
        if (!_dashing) {
            setXVelocity(moveSpeed);
            if (playerSpriteRenderer.flipX) {
                playerSpriteRenderer.flipX = false;
            }
        }
        if (currentAbility.Equals(ActiveAbility.FIRE))
        {
            horizontalDashDirection = 1;
        }
    }

    void stopMoving()
    {
        if (!_dashing) {
            setXVelocity(0.0f);
        }
        if (currentAbility.Equals(ActiveAbility.FIRE))
        {
            horizontalDashDirection = 0;
        }
    }

    void stopClimbing()
    {
        _isClimbing = false;
        if (_isHuggingWall)
        {
            setYVelocity(0.0f);
        }
        if (currentAbility.Equals(ActiveAbility.FIRE))
        {
            verticalDashDirection = 0;
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
        if (_isGrounded && !currentAbility.Equals(ActiveAbility.ICE))
        {
            playerRigidBody.AddForce(Vector2.up*jumpSpeed, ForceMode2D.Impulse);
        }
        else if (_isHuggingWall && currentAbility.Equals(ActiveAbility.EARTH))
        {
            stopHuggingWall();
            playerRigidBody.AddForce(Vector2.up*jumpSpeed, ForceMode2D.Impulse);
        }
        else if (_canDash && !_isGrounded && (horizontalDashDirection != 0 || verticalDashDirection != 0) && currentAbility.Equals(ActiveAbility.FIRE))
        {
            startDash();
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
            GetComponent<SpriteRenderer>().color = Color.blue;
            //May need to add ice constants for these properties.
            moveSpeed = ICE_MOVEMENT_SPEED;
            jumpSpeed = NORMAL_JUMP_SPEED;
            playerRigidBody.mass = NORMAL_MASS;
            if (!_isGrounded) {
                //have player fall very fast if airborne when switching to ice
                playerRigidBody.AddForce(Vector2.down * ICE_FALL_MAGNITUDE, ForceMode2D.Impulse);
            }
        }
        else
        {
            deactivateAbility();
        }
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
            moveSpeed = NORMAL_MOVEMENT_SPEED;
            jumpSpeed = NORMAL_JUMP_SPEED;
            playerRigidBody.mass = NORMAL_MASS;
        }
        else
        {
            deactivateAbility();
        }
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
            moveSpeed = NORMAL_MOVEMENT_SPEED;
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
            moveSpeed = NORMAL_MOVEMENT_SPEED;
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
    }

    void deactivateAbility()
    {
        moveSpeed = NORMAL_MOVEMENT_SPEED; //In anticipation for ice slowing movement down.
        jumpSpeed = NORMAL_JUMP_SPEED;
        playerRigidBody.mass = NORMAL_MASS;
        currentAbility = ActiveAbility.NORMAL;
        GetComponent<SpriteRenderer>().color = Color.white;
    }


    void startDash()
    {
        _dashing = true;
        _canDash = false;

        //add extra upwards force to push against gravity
        playerRigidBody.gravityScale = 0;
        playerRigidBody.velocity = new Vector2(horizontalDashDirection * fireDashSpeed, verticalDashDirection * fireDashSpeed);
        Invoke("stopDash", FIRE_DASH_DURATION);
    }

    void stopDash() {
        _dashing = false;
        playerRigidBody.gravityScale = 1;
        //Player starts free-falling once dash has stopped.
        float yVelocityAfterDash = playerRigidBody.velocity.y;
        if (playerRigidBody.velocity.y > 0) {
            yVelocityAfterDash = 0;
        }
        setXVelocity(0.0f);
        setYVelocity(yVelocityAfterDash); //Maybe set to 0 in all cases? Check again once fire animations are implemented.
    }

    public void hazardHitsPlayer(bool breaksIceArmor)
    {
        if (breaksIceArmor || !currentAbility.Equals(ActiveAbility.ICE))
        {
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        //TODO: Handle player death.
        Destroy(this.gameObject);
        Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_OVER); //Simulates player respawn until checkpoints have been implemented.
    }
}
