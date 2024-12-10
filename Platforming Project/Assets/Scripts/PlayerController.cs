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

    //tracks the players movement
    public Vector2 currentPosition;
    //A veribale that will determin weather the player can dash
    bool hasDashed;
    //speed that is added to the players dash
    public float dashingForce = 25f;

    //Task 2 assignment 2

    //the first timer that will count up when tab is held down
    public float time;
    //this is the varrible that checks if the jump can be excicuted
    bool chargeJumping;
    //a second verrible that will start to tick upwards when the time = 3. this will rest everthing back to narmal after 2 seconds
    public float time2;

    //Task 3 assignment 3

    //this is to sense if the player is on the spring layer
    public LayerMask Spring;
    private bool onSpring = false;


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

        // the player will dash if has dashed is false and shift is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift) && hasDashed == false)
            Dash();

        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = 0;

        body.velocity = velocity;

       //when the player is on the ground they can not dash
        if (isGrounded == true)
        {
            hasDashed = false;
        }

        // if the player is in the air and shift is pressed the player will dash
        if(Input.GetKeyDown(KeyCode.LeftShift) && !isGrounded)
        {
            hasDashed = true;   

            Debug.Log("we jumping");
        }
        //calls dash
        Dash();

        //code for task 2 assignment 2

        //when is is held down and the player is on the ground time will start gaining value
        if (Input.GetKey(KeyCode.Tab) && isGrounded)
        {
            //chargeJumping = true;

            time += Time.deltaTime;
        }
        //calls chargeJump
        ChargeJump();

        //if the charge jump is true then time2 will start gaining value
        if( chargeJumping == true)
        {
            time2 += Time.deltaTime;
        }

        //once time 2 hits 3 it sets charge jump false and time2 back to 0/
        if (time2 >= 3)
        {
            chargeJumping = false;  
            time2 = 0;
        }

        //Task 3 assignment 2

        //calls checkForSpring methode
        CheckForSpring();

        //this was supposed to for to send the player up when onSpring is true.
        if (onSpring)
        {
            velocity.y += 3;
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

    // task 3 assignment 2

    //this should have sent the OnSpring to true when the players overlapbox sences the Spring layer.
    private void CheckForSpring()
    {
        onSpring = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize, 0, Spring);
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
    public bool OnSpring()
    {
        return onSpring;
    }

    public PlayerDirection GetFacingDirection()
    {
        return currentDirection;
    }

    //task 1
    private void Dash()
    {
        //these lines of codes will send the player hurtaling in the direction they are fasing 
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

        //after the code above has run has dashed is reset.
        hasDashed = false;

    }

    //task 2
    private void ChargeJump()
    {

        //when time which counts up when tab is held down hits 3 it will change the initialjumpspeed to 30 so that when jump is ready the player can press space and there jump will 
        //be much higher
        if(time >= 3f  )
        {
            chargeJumping = true;

            initialJumpSpeed = 30f;

            Debug.Log("JUMP READY");

            //when time2 is at 2 or higher is will reset the initialjumpspeed and set time back to 0.
            if (time2 >= 2)
            {
                initialJumpSpeed = 2 * apexHeight / apexTime;

                time = 0;
            }
        }

    }
}
