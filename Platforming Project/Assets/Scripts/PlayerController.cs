using UnityEngine;

public enum PlayerDirection
{
    left, right
}

public enum PlayerState
{
    idle, walking, jumping, dead
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    private PlayerDirection currentDirection = PlayerDirection.right;
    public PlayerState currentState = PlayerState.idle;
    public PlayerState previousState = PlayerState.idle;

    [Header("Horizontal")]
    public float maxSpeed = 5f;
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.15f;

    [Header("Vertical")]
    public float apexHeight = 3f;
    public float apexTime = 0.5f;

    [Header("Ground Checking")]
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    private float accelerationRate;
    private float decelerationRate;

    private float gravity;
    private float initialJumpSpeed;

    private bool isGrounded = false;
    public bool isDead = false;

    private Vector2 velocity;

    //Task 1 assingment 2
    public Vector2 currentPosition;
    bool hasDashed;
    public float dashingForce = 25f;

    //Task 2 assignment 2
    public float time;
    bool chargeJumping;
    public float time2;

    public void Start()
    {
        body.gravityScale = 0;

        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpSpeed = 2 * apexHeight / apexTime;

    }

    public void Update()
    {
        previousState = currentState;

        currentPosition = transform.position;

        CheckForGround();

        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");

        if (isDead)
        {
            currentState = PlayerState.dead;
        }

        switch(currentState)
        {
            case PlayerState.dead:
                // do nothing - we ded.
                break;
            case PlayerState.idle:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (velocity.x != 0) currentState = PlayerState.walking;
                break;
            case PlayerState.walking:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (velocity.x == 0) currentState = PlayerState.idle;
                break;
            case PlayerState.jumping:
                if (isGrounded)
                {
                    if (velocity.x != 0) currentState = PlayerState.walking;
                    else currentState = PlayerState.idle;
                }
                break;
        }

        MovementUpdate(playerInput);
        JumpUpdate();

        //task 1 for dashing
        if (Input.GetKeyDown(KeyCode.LeftShift) && hasDashed == false)
            Dash();

        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = 0;

        body.velocity = velocity;

        // the player cant dash when they are on the ground
        if (isGrounded == true)
        {
            hasDashed = false;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && !isGrounded)
        {
            hasDashed = true;   

            Debug.Log("we jumping");
        }

        Dash();

        //code for task 2 assignment 2

        if (Input.GetKey(KeyCode.Tab) && isGrounded)
        {
            //chargeJumping = true;

            time += Time.deltaTime;
        }

        ChargeJump();

        if( chargeJumping == true)
        {
            time2 += Time.deltaTime;
        }

        if (time2 >= 3)
        {
            chargeJumping = false;  
            time2 = 0;
        }
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput.x < 0)
            currentDirection = PlayerDirection.left;
        else if (playerInput.x > 0)
            currentDirection = PlayerDirection.right;

        if (playerInput.x != 0)
        {
            velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            if (velocity.x > 0)
            {
                velocity.x -= decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0)
            {
                velocity.x += decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }
    }

    private void JumpUpdate()
    {
        if (isGrounded && Input.GetButton("Jump"))
        {
            velocity.y = initialJumpSpeed;
            isGrounded = false;
        }
    }

    private void CheckForGround()
    {
        isGrounded = Physics2D.OverlapBox(
            transform.position + Vector3.down * groundCheckOffset,
            groundCheckSize,
            0,
            groundCheckMask);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
    }

    public bool IsWalking()
    {
        return velocity.x != 0;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public PlayerDirection GetFacingDirection()
    {
        return currentDirection;
    }

    private void Dash()
    {

        if (hasDashed && currentDirection == PlayerDirection.left)
        {
            velocity.x -= velocity.x + dashingForce;
            Debug.Log("Dashing left");
        }
        else if (hasDashed && currentDirection == PlayerDirection.right)
        {
            velocity.x += velocity.x + dashingForce;
            Debug.Log("Dashing right");
        }

        hasDashed = false;

    }


    private void ChargeJump()
    {
        if(time >= 3f  )
        {
            chargeJumping = true;

            initialJumpSpeed = 30f;

            Debug.Log("JUMP READY");


            if (time2 >= 2)
            {
                initialJumpSpeed = 2 * apexHeight / apexTime;

                time = 0;
            }
        }

    }
}
