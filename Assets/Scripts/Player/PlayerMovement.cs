using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Other;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class PlayerMovement : MonoBehaviour
{
    //Scriptable object which holds all the player's movement parameters. If you don't want to use it
    //just paste in all the parameters, though you will need to manuly change all references in this script
    public PlayerData Data;
    public bool allowPlayerMovement;
    public float runMaxSpeed;
    public float runMaxSpeedNoPet;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    //Script to handle all player animations, all references can be safely removed if you're importing into your own project.
    public PlayerAnimator AnimHandler { get; private set; }
    #endregion

    #region STATE PARAMETERS
    //Variables control the various actions the player can perform at any time.
    //These are fields which can are public allowing for other sctipts to read them
    //but can only be privately written to.
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsDashing { get; private set; }
    public bool IsSliding { get; private set; }

    //Timers (also all fields, could be private and a method returning a bool could be used)
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;

    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    //Dash
    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    private bool _isDashAttacking;

    #endregion

    #region INPUT PARAMETERS
    public Vector2 _moveInput;
    public bool lookingRight;

    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    //Set all of these up in the inspector
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        AnimHandler = GetComponent<PlayerAnimator>();
        currentTimeScale = Time.timeScale;
        previousJumpInFuture = 69;
    }
    //bool timeSlowed;

    float currentTimeScale;
    public float slowedGravity = 4;

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
        //timeSlowed = false;
    }

    void MoveDirectionCheck()
    {
        if (Mathf.Abs(_moveInput.x) > 0)
            lookingRight = _moveInput.x == 1;
    }

    public void ToggleTime()
    {
        //if (GameManager.Instance.playerController.petAbality.isThrown) return;

        //timeSlowed = !timeSlowed;
        //currentTimeScale = timeSlowed ? 0.5f : 1;
        //Time.timeScale = currentTimeScale;
    }

    void printTimeStamp()
    {
        foreach (var t in InputRecorder.i.moveInputHistory)
        {
            print(t.Item1 + " : " + t.Item2);
        }
    }

    [Header("Time")]
    public bool isReplayingFuture;
    private Vector2 previousMoveInput = new(6, 9);
    public float startTimeInFutureWhenInFuture = 0f;
    private int currentReplayIndex = 0;
    private int currentJumpReplayIndex = 0;
    bool setOnceReplay;
    bool setOnceReplayForJump;
    float timePassed;

    float nextReplayTime;
    float nextJumpReplayTime;

    bool timeOnce;
    bool timeOnce2;
    bool timeJumpOnce;

    int isJumpingInFuture;
    int previousJumpInFuture = 69;

    int isJumpingInPresentTimeFutureVarient;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            printTimeStamp();
        }

        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;
        #endregion

        MoveDirectionCheck();

        #region INPUT HANDLER
        if (!isReplayingFuture)
        {
            if (allowPlayerMovement)
            {
                _moveInput.x = Input.GetAxisRaw("Horizontal");
                _moveInput.y = Input.GetAxisRaw("Vertical");
            }
            else
            {
                _moveInput.x = 0;
                _moveInput.y = 0;
            }
        }

        #region Replay Future Actions

        else
        {
            // Replay recorded inputs based on the timeline
            if (currentReplayIndex < InputRecorder.i.moveInputHistory.Count)
            {
                // Get the recorded input and time from the history
                Vector2 recordedInput = InputRecorder.i.moveInputHistory[currentReplayIndex].Item1;
                float recordedTime = InputRecorder.i.moveInputHistory[currentReplayIndex].Item2;

                if (!timeOnce2)
                {
                    transform.position = InputRecorder.i.PlayerPosRec;
                    timeOnce2 = true;
                }

                if (!setOnceReplay)
                {
                    nextReplayTime = Time.time + recordedTime;
                    setOnceReplay = true;
                }

                // Check if the time passed since the start of replaying is greater than the recorded time
                if (Time.time < nextReplayTime)
                {
                    _moveInput = recordedInput;
                }
                else
                {
                    // Time has passed, freeze _moveInput
                    //_moveInput = recordedInput;
                    _moveInput = Vector2.zero;
                    setOnceReplay = false;
                    currentReplayIndex++;
                }
            }
            else
            {
                // Finished replaying, stop replay mode
                isReplayingFuture = false;
                currentReplayIndex = 0;
                timeOnce2 = false;

                InputRecorder.i.calledFutureInstance = false;
                InputRecorder.i.startTimeInPresent = 340282346638528859811704183484516925440.000000f;
                InputRecorder.i.PlayerMultipleFutureInstancePos.Clear();
                InputRecorder.i.jumpInputHistory.Clear();
                InputRecorder.i.PlayerMultipleFutureInstanceChangeIndex = 0;
                InputRecorder.i.zinInstance.SetActive(false);
                Destroy(gameObject);
            }
        }

        #endregion

        MoveDirectionCheck();

        //_moveInput.x = Joystick.Instance.GetMovementInput().x;
        //_moveInput.y = Joystick.Instance.GetMovementInput().y;

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);

        if (!isReplayingFuture)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJumpingInFuture = 1;
                OnJumpInput();
            }
            else isJumpingInFuture = 0;

            if (Input.GetKeyUp(KeyCode.Space))
            {
                OnJumpUpInput();
            }

            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K))
            {
                OnDashInput();
            }
        }
        else
        {
            // Replay recorded inputs based on the timeline
            if (currentJumpReplayIndex < InputRecorder.i.jumpInputHistory.Count)
            {
                // Get the recorded input and time from the history
                int recordedJumpInput = InputRecorder.i.jumpInputHistory[currentJumpReplayIndex].Item1;
                float recordedTime = InputRecorder.i.jumpInputHistory[currentJumpReplayIndex].Item2;

                if (!timeJumpOnce)
                {
                    isJumpingInPresentTimeFutureVarient = 0;
                    timeJumpOnce = true;
                }

                if (!setOnceReplayForJump)
                {
                    nextJumpReplayTime = Time.time + recordedTime;
                    setOnceReplayForJump = true;
                }

                //print("every frame : " + isJumpingInPresentTimeFutureVarient);

                // Check if the time passed since the start of replaying is greater than the recorded time
                if (Time.time < nextJumpReplayTime)
                {
                    isJumpingInPresentTimeFutureVarient = recordedJumpInput;
                    print($"Playing : {isJumpingInPresentTimeFutureVarient} : {recordedTime}");
                    if (isJumpingInPresentTimeFutureVarient == 1)
                    {
                        OnJumpInput();
                    }
                    else
                    {
                        OnJumpUpInput();
                    }
                }
                else
                {
                    //isJumpingInPresentTimeFutureVarient = false;
                    setOnceReplayForJump = false;
                    currentJumpReplayIndex++;
                }
            }
        }


        #region Future move record

        if (InputRecorder.i.inFuture)
        {
            if (InputRecorder.i.goingInMultipleFutures) return;

            float currentTime = Time.time;

            // Check if this is the first frame when inFuture becomes true
            if (startTimeInFutureWhenInFuture == 0f)
            {
                InputRecorder.i.PlayerPosRec = transform.position;
                InputRecorder.i.moveInputHistory.Clear();
                timeOnce = false;
                startTimeInFutureWhenInFuture = currentTime;
            }

            // Calculate the time passed since inFuture became true
            timePassed = currentTime - startTimeInFutureWhenInFuture;

            // Check if moveInput has changed since the previous frame
            if (_moveInput != previousMoveInput)
            {
                // Add a tuple to the list containing the moveInput and the seconds passed
                InputRecorder.i.moveInputHistory.Add((previousMoveInput, timePassed));
                startTimeInFutureWhenInFuture = currentTime;
            }

            if (isJumpingInFuture != previousJumpInFuture)
            {
                InputRecorder.i.jumpInputHistory.Add((previousJumpInFuture, timePassed));
            }

            // Update the previousMoveInput for the next frame
            previousMoveInput = _moveInput;
            previousJumpInFuture = isJumpingInFuture;
        }
        else
        {
            if (!timeOnce)
            {
                InputRecorder.i.moveInputHistory.Add((previousMoveInput, timePassed));
                InputRecorder.i.jumpInputHistory.Add((previousJumpInFuture, timePassed));
                timeOnce = true;

                // Remove the item at index 0 if it exists and the list is not empty
                if (InputRecorder.i.moveInputHistory.Count > 0 && InputRecorder.i.moveInputHistory[0].Item1 == new Vector2(6, 9))
                {
                    InputRecorder.i.moveInputHistory.RemoveAt(0);
                    //InputRecorder.i.jumpInputHistory.RemoveAt(0);
                    // Optionally sort the list if order matters
                    // moveInputHistory.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                }

                if (InputRecorder.i.jumpInputHistory.Count > 0 && InputRecorder.i.jumpInputHistory[0].Item1 == 69)
                {
                    print("removed");
                    InputRecorder.i.jumpInputHistory.RemoveAt(0);
                    // Optionally sort the list if order matters
                    // moveInputHistory.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                }
                //timeOnce2 = false;
            }
            // Reset the timer when inFuture becomes false
            startTimeInFutureWhenInFuture = 0f;
        }

        #endregion


        #endregion

        //if (Input.GetMouseButtonDown(1))
        //{
        //    ToggleTime();
        //}
        //else if (Input.GetMouseButtonUp(1))
        //{
        //    ToggleTime();
        //}

        #region COLLISION CHECKS
        if (!IsDashing && !IsJumping)
        {
            //Ground Check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping) //checks if set box overlaps with ground
            {
                if (LastOnGroundTime < -0.1f)
                {
                    AnimHandler.justLanded = true;
                }

                LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
            }

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
                LastOnWallRightTime = Data.coyoteTime;

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
                LastOnWallLeftTime = Data.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }
        #endregion

        #region JUMP CHECKS
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false;
            FindObjectOfType<AudioManager>().Play("jump");

            if (!IsWallJumping)
                _isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;

            if (!IsJumping)
                _isJumpFalling = false;
        }

        if (!IsDashing)
        {
            //Jump
            if (CanJump() && LastPressedJumpTime > 0)
            {
                IsJumping = true;
                IsWallJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;
                Jump();

                AnimHandler.startedJumping = true;
            }
            //WALL JUMP
            else if (CanWallJump() && LastPressedJumpTime > 0)
            {
                IsWallJumping = true;
                IsJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;

                _wallJumpStartTime = Time.time;
                _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

                WallJump(_lastWallJumpDir);
            }
        }
        #endregion

        #region DASH CHECKS
        if (CanDash() && LastPressedDashTime > 0)
        {
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            Sleep(Data.dashSleepTime);

            //If not direction pressed, dash forward
            if (_moveInput != Vector2.zero)
                _lastDashDir = _moveInput;
            else
                _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;



            IsDashing = true;
            IsJumping = false;
            IsWallJumping = false;
            _isJumpCut = false;

            StartCoroutine(nameof(StartDash), _lastDashDir);
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
            IsSliding = true;
        else
            IsSliding = false;
        #endregion

        #region GRAVITY
        if (!_isDashAttacking)
        {
            //Higher gravity if we've released the jump input or are falling
            if (IsSliding)
            {
                SetGravityScale(0);
            }
            else if (RB.velocity.y < 0 && _moveInput.y < 0)
            {
                //Much higher gravity if holding down
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
            }
            else if (_isJumpCut)
            {
                //Higher gravity if jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (RB.velocity.y < 0)
            {
                //Higher gravity if falling
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            //No gravity when dashing (returns to normal once initial dashAttack phase over)
            SetGravityScale(0);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        //Handle Run
        if (!IsDashing)
        {
            if (IsWallJumping)
                Run(Data.wallJumpRunLerp);
            else
                Run(1);
        }
        else if (_isDashAttacking)
        {
            Run(Data.dashEndRunLerp);
        }

        //Handle Slide
        if (IsSliding)
            Slide();
    }

    #region INPUT CALLBACKS
    //Methods which whandle input detected in Update()
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }

    public void OnDashInput()
    {
        LastPressedDashTime = Data.dashInputBufferTime;
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        //RB.gravityScale = currentTimeScale != 1 ? slowedGravity : scale;
        RB.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        //Method used so we don't need to call StartCoroutine everywhere
        //nameof() notation means we don't need to input a string directly.
        //Removes chance of spelling mistakes and will improve error messages if any
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        //yield break;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
        Time.timeScale = 1f;
    }
    #endregion

    //MOVEMENT METHODS
    #region RUN METHODS

    public void SlowPlayerByPercent(float percent)
    {
        // Ensure the input percentage is within the valid range (0 to 100)
        percent = Mathf.Clamp(percent, 0f, 100f);

        // Calculate the new run speed based on the percentage
        Data.runMaxSpeed = runMaxSpeed * (1 - (percent / 100f));

        // Make sure runMaxSpeed doesn't go below 0
        Data.runMaxSpeed = Mathf.Max(0, Data.runMaxSpeed);
    }

    public void SetPlayerSpeed(float speed = -1)
    {
        Data.runMaxSpeed = speed < 0 ? runMaxSpeed : speed;
    }

    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed /*= currentTimeScale != 1 ? targetSpeed * 4 : targetSpeed*/, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = Data.jumpForce;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }

    private void WallJump(int dir)
    {
        //Ensures we can't call Wall Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        #region Perform Wall Jump
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
            force.x -= RB.velocity.x;

        if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= RB.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        RB.AddForce(force, ForceMode2D.Impulse);
        #endregion
    }
    #endregion

    #region DASH METHODS
    //Dash Coroutine
    private IEnumerator StartDash(Vector2 dir)
    {
        //Time.timeScale = 0.2f;
        //Overall this method of dashing aims to mimic Celeste, if you're looking for
        // a more physics-based approach try a method similar to that used in the jump

        LastOnGroundTime = 0;
        LastPressedDashTime = 0;

        float startTimeInFuture = Time.time;

        _dashesLeft--;
        _isDashAttacking = true;

        SetGravityScale(0);

        //We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
        while (Time.time - startTimeInFuture <= Data.dashAttackTime)
        {
            RB.velocity = dir.normalized * Data.dashSpeed;
            //Pauses the loop until the next frame, creating something of a Update loop. 
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            yield return null;
        }

        startTimeInFuture = Time.time;

        _isDashAttacking = false;

        //Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
        SetGravityScale(Data.gravityScale);
        RB.velocity = Data.dashEndSpeed * dir.normalized;

        while (Time.time - startTimeInFuture <= Data.dashEndTime)
        {
            yield return null;
        }

        //Dash over
        Time.timeScale = 1;
        IsDashing = false;
    }

    //Short period before the player is able to dash again
    private IEnumerator RefillDash(int amount)
    {
        //SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
        _dashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        _dashRefilling = false;
        _dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        //Works the same as the Run but only in the y-axis
        //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
        float speedDif = Data.slideSpeed - RB.velocity.y;
        float movement = speedDif * Data.slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        RB.AddForce(movement * Vector2.up);
    }
    #endregion


    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
             (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return IsJumping && RB.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && RB.velocity.y > 0;
    }

    private bool CanDash()
    {
        if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return _dashesLeft > 0;
    }

    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }
    #endregion


    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
    #endregion

    public GameObject spawnPoint;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            FindObjectOfType<AudioManager>().Play("hit");
            transform.position = spawnPoint.transform.position;
        }
    }
}