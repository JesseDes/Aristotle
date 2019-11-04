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
    float jumpSpeed, fireDashSpeed = 8.0f;

    [SerializeField]
    float climbSpeed;

    [SerializeField]
    float iceMagnitude = 20.0f;

    [HideInInspector]
    public ActiveAbility currentAbility { get; private set; }

    private const float NORMAL_MOVEMENT_SPEED = 5.0f;
    private const float NORMAL_JUMP_SPEED = 7.5f;
    private const float NORMAL_MASS = 1.0f;

    private const float WIND_JUMP_SPEED = NORMAL_JUMP_SPEED * 1.25f;
    private const float WIND_MASS = NORMAL_MASS * 0.8f;

    private const float EARTH_CLIMB_SPEED = 4.0f;

    const float FIRE_DASH_DURATION = 0.75f; //reference for how long dash maneuver from fire power lasts

    float initMoveSpeed, initJumpSpeed;


    PlayerInputProfile inputProfile;
    Rigidbody2D playerRigidBody;
    Animator animator;
    SpriteRenderer playerSpriteRenderer;

    private bool _isGrounded;
    private bool _shiftPressed, _canDash, _dashing;
    private bool _isHuggingWall;
    private bool _isClimbing;
    private bool _isFalling;
    private float _currentSpeed = 0;
    private Vector3 _storedForce;
    private bool _isRespawn = false;
    // Start is called before the first frame update

    public void init()
    {
        _isRespawn = true;
    }

    void Start()
    {
        initMoveSpeed = moveSpeed;
        initJumpSpeed = jumpSpeed;

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

        SetUpInputProfile();

        currentAbility = ActiveAbility.NORMAL;
        this.enabled = _isRespawn;
        Controller.instance.stateMachine.AddStateListener(onStateChange);

    }

    private void SetUpInputProfile()
    {
        inputProfile = new PlayerInputProfile();
        //Listeners for vertical movement.
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveRight, moveRight);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveLeft, stopMoving);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveRight, stopMoving);

        //Listeners for movement and jumping.
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveRight, moveRight);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveLeft, stopMoving);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveRight, stopMoving);

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

    }

    // Update is called once per frame
    private void Update()
    {
        inputProfile.checkInput();
    }

    void FixedUpdate()
    {
        //code for elemental power logic goes here
        //ice...
        if (currentAbility.Equals(ActiveAbility.ICE)) {
            //slow player down and lock down jump ability
            moveSpeed = initMoveSpeed / 2f; //halve(?) movement speed
            jumpSpeed = 0; //means: can't jump
        }
        else {
            moveSpeed = initMoveSpeed;
            jumpSpeed = initJumpSpeed;
        }

        //fire...
        if (currentAbility.Equals(ActiveAbility.FIRE)) {
            if (_canDash) {
                float moveH = Input.GetAxisRaw("Horizontal");
                float moveV = Input.GetAxisRaw("Vertical");

                //print(moveH + ", " + moveV);

                //dash effect from fire, restrict to only in the air for simpler implementation
                if (_shiftPressed && !_isGrounded) {
                    unpressShift();
                    _dashing = true;
                    _canDash = false;

                    //add extra upwards force to push against gravity
                    playerRigidBody.velocity = new Vector2(moveH * fireDashSpeed, moveV * fireDashSpeed + 3f);
                    Invoke("stopDash", FIRE_DASH_DURATION);
                }
            }
            else {
                if (_isGrounded) {
                    _canDash = true;
                }
            }
        }
        //earth...
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
            SetUpInputProfile();
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
        if (!_dashing) {
            setXVelocity(-moveSpeed);
            if (!playerSpriteRenderer.flipX) {
                playerSpriteRenderer.flipX = true;
            }
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
            playerRigidBody.AddForce(Vector2.up*jumpSpeed, ForceMode2D.Impulse);
        }
        else if (_isHuggingWall && currentAbility.Equals(ActiveAbility.EARTH))
        {
            stopHuggingWall();
            playerRigidBody.AddForce(Vector2.up*jumpSpeed, ForceMode2D.Impulse);
        }
        else if (currentAbility.Equals(ActiveAbility.FIRE))
        {
            shift();
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
            jumpSpeed = NORMAL_JUMP_SPEED;
            playerRigidBody.mass = NORMAL_MASS;
            if (!_isGrounded) {
                //have player fall very fast if airborne when switching to ice
                playerRigidBody.AddForce(Vector2.down * iceMagnitude, ForceMode2D.Impulse);
            }
        }
        else
        {
            currentAbility = ActiveAbility.NORMAL;
            GetComponent<SpriteRenderer>().color = Color.white;
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
    }

    void deactivateAbility()
    {
        moveSpeed = NORMAL_MOVEMENT_SPEED; //In anticipation for ice slowing movement down.
        jumpSpeed = NORMAL_JUMP_SPEED;
        playerRigidBody.mass = NORMAL_MASS;
        currentAbility = ActiveAbility.NORMAL;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void shift() {
        if (_canDash)
        {
            _shiftPressed = true;
        }
    }

    void unpressShift() {
        _shiftPressed = false;
    }

    void stopDash() {
        _dashing = false;
        //Player starts free-falling once dash has stopped.
        float resetY = playerRigidBody.velocity.y;
        if (playerRigidBody.velocity.y > 0) {
            resetY = 0;
        }
        playerRigidBody.velocity = new Vector2(
            playerRigidBody.velocity.x,
            resetY); //cancel previous change to velocity from fire dash
    }
    void canDashAgain() {
        if (_isGrounded) {
            _canDash = true;
        }
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
