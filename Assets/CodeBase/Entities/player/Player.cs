using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

public class Player : MonoBehaviour {
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

    [SerializeField]
    float pauseHoldDuration = 1.5f;

    [SerializeField]
    ActiveAbility recentlyUnlockedAbility;

    [HideInInspector]
    public ActiveAbility currentAbility { get; private set; }

    [HideInInspector]
    public Vector2 facingDireciont { get; private set; }

    [HideInInspector]
    public Vector2 windForce = Vector2.zero;


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
    private const float WIND_FALL_SPEED = -2.0f;

    private const float EARTH_CLIMB_SPEED = 4.0f;

    //Various properties of the player entity.
    PlayerInputProfile inputProfile;
    Rigidbody2D playerRigidBody;
    Animator animator;
    SpriteRenderer playerSpriteRenderer;

    private int horizontalDashDirection = 0;
    private int verticalDashDirection = 0;

    private bool _isGrounded;
    private bool _canDash;
    private bool _dashActive;
    private bool _isHuggingWall;
    private bool _isClimbing;
    private bool _isFalling;
    private float _currentSpeed = 0;
    private Vector3 _storedForce;
    private bool _isRespawn = false;
    private bool _disableMovement = false;
    private float _pauseTimer;

    public void init() {
        _isRespawn = true;
    }

    // Start is called before the first frame update
    void Start() {
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        facingDireciont = Vector2.right;
        _isGrounded = false;

        _canDash = true;
        _dashActive = false;

        _isHuggingWall = false;
        _isClimbing = false;

        moveSpeed = NORMAL_MOVEMENT_SPEED;
        jumpSpeed = NORMAL_JUMP_SPEED;
        climbSpeed = EARTH_CLIMB_SPEED;
        playerRigidBody.mass = NORMAL_MASS;

        SetUpInputProfile();

        currentAbility = ActiveAbility.NORMAL;

        if (!PlayerPrefs.HasKey(SaveKeys.ACTIVE_ABILITIES))
            PlayerPrefs.SetInt(SaveKeys.ACTIVE_ABILITIES, (int)ActiveAbility.ICE);

        recentlyUnlockedAbility = (ActiveAbility)PlayerPrefs.GetInt(SaveKeys.ACTIVE_ABILITIES);
        UpdateAbilitySymbols(ActiveAbility.NORMAL);
        this.enabled = _isRespawn;
        Controller.instance.stateMachine.AddStateListener(onStateChange);
    }

    private void UpdateAbilitySymbols(ActiveAbility ability)
    {
        View.instance.UpdateAbilitySymbol(ActiveAbility.ICE, recentlyUnlockedAbility >= ActiveAbility.ICE ? AbilitySymbolState.Unavailable : AbilitySymbolState.Locked);
        View.instance.UpdateAbilitySymbol(ActiveAbility.FIRE, recentlyUnlockedAbility >= ActiveAbility.FIRE ? AbilitySymbolState.Unavailable : AbilitySymbolState.Locked);
        View.instance.UpdateAbilitySymbol(ActiveAbility.WIND, recentlyUnlockedAbility >= ActiveAbility.WIND ? AbilitySymbolState.Unavailable : AbilitySymbolState.Locked);
        View.instance.UpdateAbilitySymbol(ActiveAbility.EARTH, recentlyUnlockedAbility >= ActiveAbility.EARTH ? AbilitySymbolState.Unavailable : AbilitySymbolState.Locked);
        View.instance.UpdateAbilitySymbol(ability, AbilitySymbolState.Available);
    }

    private void SetUpInputProfile() {
        inputProfile = new PlayerInputProfile();

        //Listeners for movement and jumping.
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveLeft, moveLeft);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveRight, moveRight);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveUp, moveUp);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.moveDown, moveDown);
        inputProfile.addListener(InputEvent.Down, PlayerInputProfile.jump, jump);
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.jump, windfall);

        //Listeners for stopping movement.
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveUp, stopVerticalMovement);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveDown, stopVerticalMovement);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveLeft, stopHorizontalMovement);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.moveRight, stopHorizontalMovement);

        //Listeners for abilities.
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleIce, toggleIce);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleFire, toggleFire);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleWind, toggleWind);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.toggleEarth, toggleEarth);

        //Listener for Pause
        inputProfile.addListener(InputEvent.Key, PlayerInputProfile.pause, PauseDown);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.pause, PauseReleased);

        Camera.main.GetComponent<LevelCamera>().panStartEvent.AddListener(ControlStateChange);
        Camera.main.GetComponent<LevelCamera>().panCompleteEvent.AddListener(ControlStateChange);

    }

    private void ControlStateChange(System.Object response) {

        _disableMovement = !_disableMovement;
    }

    // Update is called once per frame
    private void Update() {
        if (!_disableMovement)
            inputProfile.checkInput();
    }

    void FixedUpdate() {
        if (playerRigidBody.velocity.y >= 0.1) // player is jumping
        {
            _isFalling = false;
        }
        else if (playerRigidBody.velocity.y < -0.1) // player is falling
        {
            _isFalling = true;
        }

        // Animate player movement
        animator.SetFloat("speed", moveSpeed * Mathf.Abs(playerRigidBody.velocity.x));
        animator.SetBool("isGrounded", _isGrounded);
        animator.SetBool("isFalling", _isFalling);
        animator.SetInteger("AbilityCode", (int)currentAbility);
        animator.SetBool("isHuggingWall", _isHuggingWall);
        animator.SetBool("isClimbing", _isClimbing);
    }

    private void onStateChange(System.Object response) {
        if (Controller.instance.stateMachine.state == EngineState.MENU) {            
            _storedForce = playerRigidBody.velocity;
            playerRigidBody.Sleep();
            inputProfile.DisableInput();

        }
        else if (Controller.instance.stateMachine.state == EngineState.ACTIVE) {
            inputProfile.EnableInput();
            playerRigidBody.WakeUp();
            playerRigidBody.velocity = new Vector2(0,_storedForce.y);
        }
        else if (Controller.instance.stateMachine.state == EngineState.CUTSCENES)
        {
            this.enabled = false;
            playerRigidBody.Sleep();
        }
    }

    public void OnDestroy() {
        Controller.instance.stateMachine.RemoveStateListener(onStateChange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EarthWall"))
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
        if (collision.gameObject.CompareTag("MetalMaterial"))
        {
            if (currentAbility.Equals(ActiveAbility.ICE))
            {
                //have player fall very fast if airborne when going off edge while ice is active.
                playerRigidBody.AddForce(Vector2.down * iceMagnitude, ForceMode2D.Impulse);
            }
        }

        if (collision.gameObject.CompareTag("EarthWall"))
        {
            stopHuggingWall();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 raycastDirection = new Vector2(0, -1);
        RaycastHit2D[] raycastCollisions = Physics2D.RaycastAll(transform.position, raycastDirection, 0.7f);
        bool hitGround = false;
        for (int i = 0; i < raycastCollisions.Length; i++)
        {
            if (raycastCollisions[i].collider.CompareTag("MetalMaterial"))
            {
                hitGround = true;
            }
        }
        if (hitGround)
        {
            //Setting the free-fall velocity to 0 prevents boosted jumps at corners.
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0.0f);
            _isGrounded = true;
            _canDash = true;
            _isFalling = false;
        }
        else
        {
            _isGrounded = false;
        }
    }

    void moveUp()
    {
        verticalDashDirection = 1;

        if (_isHuggingWall && currentAbility.Equals(ActiveAbility.EARTH))
        {
            _isClimbing = true;
            setYVelocity(climbSpeed);
            animator.SetFloat("ClimbingSpeed", verticalDashDirection);
        }
    }

    void moveDown()
    {
        verticalDashDirection = -1;

        if (_isHuggingWall && currentAbility.Equals(ActiveAbility.EARTH))
        {
            _isClimbing = true;
            setYVelocity(-climbSpeed);
            animator.SetFloat("ClimbingSpeed", verticalDashDirection);
        }
    }

    void moveLeft() {
        if (!_dashActive) {
            if (currentAbility != ActiveAbility.ICE && currentAbility != ActiveAbility.WIND && !_isHuggingWall)
                setXVelocity(-moveSpeed + windForce.x);
            else if (currentAbility == ActiveAbility.WIND)
                setXVelocity(-moveSpeed + (windForce.x * 1.5f));
            else
                setXVelocity(-moveSpeed);

            if (!playerSpriteRenderer.flipX) {
                facingDireciont = Vector2.left;
                playerSpriteRenderer.flipX = true;
            }
        }
        horizontalDashDirection = -1;
    }

    void moveRight() {
        if (!_dashActive) {
            if (currentAbility != ActiveAbility.ICE && currentAbility != ActiveAbility.WIND && !_isHuggingWall)
                setXVelocity(moveSpeed + windForce.x);
            else if(currentAbility == ActiveAbility.WIND)
                setXVelocity(moveSpeed + (windForce.x * 1.5f));
            else
                setXVelocity(moveSpeed);

            if (playerSpriteRenderer.flipX) {
                facingDireciont = Vector2.right;
                playerSpriteRenderer.flipX = false;
            }
        }
        horizontalDashDirection = 1;
    }

    void jump() {
        if (_isGrounded && !currentAbility.Equals(ActiveAbility.ICE)) {
            playerRigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
        else if (_isHuggingWall && currentAbility.Equals(ActiveAbility.EARTH)) {
            stopHuggingWall();
            setYVelocity(0.0f); //Prevents an enhanced jump while climbing upwards.
            playerRigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
        //Do not waste the dash if the player has not specified a direction.
        else if (_canDash && !_isGrounded && (horizontalDashDirection != 0 || verticalDashDirection != 0) && currentAbility.Equals(ActiveAbility.FIRE)) {
            startDash();
        }
    }

    void windfall()
    {
        if (_isFalling && currentAbility.Equals(ActiveAbility.WIND))
        {
            setYVelocity(WIND_FALL_SPEED);
        }
    }

    void stopHorizontalMovement()
    {
        if (!_dashActive) {
            setXVelocity(0.0f);
        }
        horizontalDashDirection = 0;
    }

    void stopVerticalMovement() {
        _isClimbing = false;
        if (_isHuggingWall) {
            setYVelocity(0.0f);
        }
        verticalDashDirection = 0;
    }

    void startDash() {
        _dashActive = true;
        _canDash = false;
        animator.SetTrigger("Dash");
        Model.instance.audioManager.PlaySound(Model.instance.globalAudio.profileKey, "player_dash");
        //add extra upwards force to push against gravity
        playerRigidBody.gravityScale = 0;
        playerRigidBody.velocity = new Vector2(horizontalDashDirection * fireDashSpeed, verticalDashDirection * fireDashSpeed);
        RotatePlayer();
        Invoke("stopDash", FIRE_DASH_DURATION);
    }

    void stopDash() {
        if (_dashActive) //Dash could have been cancelled by switching abilities during a dash.
        {
            _dashActive = false;
            playerRigidBody.gravityScale = 1;

            //Player starts free-falling once dash has stopped.
            float yVelocityAfterDash = playerRigidBody.velocity.y;
            if (playerRigidBody.velocity.y > 0) {
                yVelocityAfterDash = 0;
            }
            setXVelocity(0.0f);
            setYVelocity(yVelocityAfterDash); //Maybe set to 0 in all cases? Check again once fire animations are implemented.
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
        }
    }

    void startHuggingWall() {
        _isHuggingWall = true;
        playerRigidBody.gravityScale = 0;
    }

    void stopHuggingWall() {
        _isHuggingWall = false;
        _isClimbing = false;
        playerRigidBody.gravityScale = 1;
    }

    void setXVelocity(float newXVelocity) {
        playerRigidBody.velocity = new Vector2(newXVelocity, playerRigidBody.velocity.y);
    }

    void setYVelocity(float newYVelocity) {
        playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, newYVelocity);
    }

    //NOTE: Changing the sprite color is a temporary measure until proper animations are
    //implemented.
    void toggleIce() {
        if (recentlyUnlockedAbility >= ActiveAbility.ICE) {
            deactivateSpecificAbility(currentAbility);
            if (!currentAbility.Equals(ActiveAbility.ICE)) {
                UpdateAbilitySymbols(ActiveAbility.ICE);
                currentAbility = ActiveAbility.ICE;
                //May need to add ice constants for these properties.
                moveSpeed = ICE_MOVEMENT_SPEED;
                jumpSpeed = NORMAL_JUMP_SPEED;
                playerRigidBody.mass = NORMAL_MASS;
                if (!_isGrounded) {
                    //have player fall very fast if airborne when switching to ice
                    playerRigidBody.AddForce(Vector2.down * ICE_FALL_MAGNITUDE, ForceMode2D.Impulse);
                }
            }
            else {
                UpdateAbilitySymbols(ActiveAbility.NORMAL);
                setAbilityToNormal();
            }
        }
    }

    void toggleFire() {
        if (recentlyUnlockedAbility >= ActiveAbility.FIRE) {
            deactivateSpecificAbility(currentAbility);
            if (!currentAbility.Equals(ActiveAbility.FIRE)) {
                UpdateAbilitySymbols(ActiveAbility.FIRE);
                currentAbility = ActiveAbility.FIRE;
                moveSpeed = NORMAL_MOVEMENT_SPEED;
                jumpSpeed = NORMAL_JUMP_SPEED;
                playerRigidBody.mass = NORMAL_MASS;
            }
            else {
                UpdateAbilitySymbols(ActiveAbility.NORMAL);
                setAbilityToNormal();
            }
        }
    }

    void toggleWind() {
        if (recentlyUnlockedAbility >= ActiveAbility.WIND) {
            deactivateSpecificAbility(currentAbility);
            if (!currentAbility.Equals(ActiveAbility.WIND)) {
                UpdateAbilitySymbols(ActiveAbility.WIND);
                currentAbility = ActiveAbility.WIND;
                moveSpeed = NORMAL_MOVEMENT_SPEED;
                jumpSpeed = WIND_JUMP_SPEED;
                playerRigidBody.mass = WIND_MASS;
            }
            else {
                UpdateAbilitySymbols(ActiveAbility.NORMAL);
                setAbilityToNormal();
            }
        }
    }

    void toggleEarth() {
        if (recentlyUnlockedAbility >= ActiveAbility.EARTH) {
            deactivateSpecificAbility(currentAbility);
            if (!currentAbility.Equals(ActiveAbility.EARTH)) {
                UpdateAbilitySymbols(ActiveAbility.EARTH);
                currentAbility = ActiveAbility.EARTH;
                moveSpeed = NORMAL_MOVEMENT_SPEED;
                jumpSpeed = NORMAL_JUMP_SPEED;
                playerRigidBody.mass = NORMAL_MASS;
                if (_isHuggingWall) {
                    startHuggingWall();
                    playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0.0f);
                }
            }
            else {
                UpdateAbilitySymbols(ActiveAbility.NORMAL);
                setAbilityToNormal();
            }
        }
    }

    void setAbilityToNormal() {
        moveSpeed = NORMAL_MOVEMENT_SPEED;
        jumpSpeed = NORMAL_JUMP_SPEED;
        playerRigidBody.mass = NORMAL_MASS;
        currentAbility = ActiveAbility.NORMAL;
    }

    void deactivateSpecificAbility(ActiveAbility specifiedAbility) {
        if (specifiedAbility.Equals(ActiveAbility.EARTH)) {
            stopHuggingWall();
        }
        else if (specifiedAbility.Equals(ActiveAbility.FIRE)) {
            stopDash();
        }
    }

    public void hazardHitsPlayer(bool breaksIceArmor) {
        if (breaksIceArmor || !currentAbility.Equals(ActiveAbility.ICE)) {
            KillPlayer();
        }
    }

    public bool isHuggingWall()
    {
        return _isHuggingWall;
    }

    private void PauseDown()
    {
        if (Controller.instance.stateMachine.state == EngineState.ACTIVE)
        {
            _pauseTimer += Time.deltaTime;
            if (_pauseTimer >= pauseHoldDuration)
            {
                Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_PAUSE);
                _pauseTimer = 0;
                Destroy(gameObject);
            }
        }
    }

    private void PauseReleased()
    {
        _pauseTimer = 0;
    }


    public void KillPlayer()
    {
        //TODO: Handle player death.
        Destroy(this.gameObject);
        Model.instance.audioManager.PlaySound(Model.instance.globalAudio.profileKey, "player_death");
        Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_OVER); 
    }

    void RotatePlayer()
    {
        float z = 0;

        // side dash
        if (verticalDashDirection == 0)
        {
            return;
        }

        // diagonal dash
        if (horizontalDashDirection == 1)
        {
            z = verticalDashDirection == 1 ? 45 : -45;
        }
        else if (horizontalDashDirection == -1)
        {
            z = verticalDashDirection == 1 ? -45 : 45;
        }
        // up or down dash
        else if (horizontalDashDirection == 0)
        {
            if (playerSpriteRenderer.flipX)
            {
                z = verticalDashDirection == 1 ? -90 : 90;
            }
            else
            {
                z = verticalDashDirection == 1 ? 90 : -90;
            }
        }

        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, z);
    }
}