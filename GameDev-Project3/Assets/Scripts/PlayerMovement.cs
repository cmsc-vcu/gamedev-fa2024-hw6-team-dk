using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [Header("References")]
    public PlayerMovementStats MoveStats;

    [SerializeField] private Collider2D feetColl;
    [SerializeField] private Collider2D bodyColl;

    private Rigidbody2D rb;

    //movement variables
    private Vector2 moveVelocity;
    private bool isFacingRight;

    //collision checking
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    public bool isThisGrounded;
    private bool hasHitHead;

    // jump variables
    public float VerticalVelocity { get; private set; }
    public bool isJumping;
    public bool isFastFalling;
    public bool isFalling;
    public float fastFallTime;
    public float fastFallReleaseSpeed;
    public int numberOfJumps;

    // Jump Extras
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;
    private float jumpbufferTimer;
    private bool jumpReleaseDuringBuffer;
    private float coyoteTimer;

    private void Awake() {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {

        CountTimers();
        JumpChecks();
    }

    private void FixedUpdate() {

        CollisionChecks();
        Jump();

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        if (isThisGrounded) {
            Move(MoveStats.GroundAcceleraition, MoveStats.GroundDeceleration, movement );
        }
        else {
            Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, movement);
        }
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput) {
        if (moveInput != Vector2.zero) {

            TurnCheck(moveInput);
            
            Vector2 targetVelocity = Vector2.zero;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxRunSpeed;
                Debug.Log("running");
            }
            else { 
                targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxWalkSpeed; 
                Debug.Log("walking");
            }

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
        }

        else if (moveInput == Vector2.zero) {

            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
        }

    }

    private void TurnCheck(Vector2 moveInput) {

        if (isFacingRight && moveInput.x < 0) {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0) {
            Turn(true);
        }
    }

    private void Turn(bool turnRight) {

        if (turnRight) {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    #endregion

    #region Jump

    private void JumpChecks() {

        // when we press the jump button
        if (Input.GetKeyDown(KeyCode.Space)) {
            jumpbufferTimer = MoveStats.JumpBufferTime;
            jumpReleaseDuringBuffer = false;
        }

        // when we release the jump button
        if (Input.GetKeyUp(KeyCode.Space)) {
            Debug.Log("test");

            if (jumpbufferTimer > 0f) {
                jumpReleaseDuringBuffer = true;
            }

            if (isJumping && VerticalVelocity > 0f) {

                if (isPastApexThreshold) {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else {
                    isFastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        // initiating jump
        if (jumpbufferTimer > 0f && !isJumping && (isThisGrounded || coyoteTimer > 0f)) {
            InitiateJump(1);

            if (jumpReleaseDuringBuffer) {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        // double jump
        else if (jumpbufferTimer > 0f && isJumping && numberOfJumps < MoveStats.NumberOfJumps) {

            isFastFalling = false;
            InitiateJump(1);
        }

        // air jump
        else if (jumpbufferTimer > 0f && isFalling && numberOfJumps < MoveStats.NumberOfJumps - 1) {

            InitiateJump(2);
            isFastFalling = false;
        }

        //landing
        if ((isJumping || isFalling) && isThisGrounded && VerticalVelocity <= 0f) {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumps = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int numberOfJumpsUsed) {
        if (!isJumping) {
            isJumping = true;
        }

        jumpbufferTimer = 0f;
        numberOfJumps += numberOfJumpsUsed;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }

    private void Jump() {

        // apply gravity
        if (isJumping) {
            if (hasHitHead) {
                isFastFalling = true;
            }

            // upwards
            if (VerticalVelocity >= 0f) {

                apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);
                if (apexPoint > MoveStats.ApexThreshold) {

                    if (!isPastApexThreshold) {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if (isPastApexThreshold) {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < MoveStats.ApexHangTime) {
                            VerticalVelocity = 0f;
                        }
                        else { VerticalVelocity = -0.01f; }
                    }
                }
                else {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold) {
                        isPastApexThreshold = false;
                    }
                }

            }

            // downwards
            else if (!isFastFalling) {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (VerticalVelocity < 0f) {
                if (!isFalling) {
                    isFalling = true;
                }
            }

        }

        //jump cut
        if (isFastFalling) {

            if (fastFallTime >= MoveStats.TimeForUpwardsCancel) {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < MoveStats.TimeForUpwardsCancel) {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, fastFallTime / MoveStats.TimeForUpwardsCancel);
            }
            fastFallTime += Time.fixedDeltaTime;
        }

        // normal gravity
        if (!isThisGrounded && !isJumping) {
            if (!isFalling) {
                isFalling = true;
            }

            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }

        // fall speed clamp
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);

        rb.velocity = new Vector2(rb.velocity.x, VerticalVelocity);
    }

    #endregion

    #region Collision Checks

    private void isGrounded() {

        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x, MoveStats.GroundDetectionrayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionrayLength, MoveStats.Groundlayer);
        if (groundHit.collider != null) {
            isThisGrounded = true;
        }
        else { isThisGrounded = false; }

    }

    private void BumpedHead() {

        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionrayLength);
        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionrayLength, MoveStats.Groundlayer);
        if (headHit.collider != null) {
            hasHitHead = true;
        }
        else { hasHitHead = false; }
        }

    private void CollisionChecks() {
        isGrounded();
        BumpedHead();
    }

    #endregion

    #region Timers

    private void CountTimers() {
        jumpbufferTimer -= Time.deltaTime;

        if (!isThisGrounded) {
            coyoteTimer -= Time.deltaTime;
        }
        else { coyoteTimer = MoveStats.JumpCayoteTime; }
    }


    #endregion


}
