using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float maxSpeed;
    public float apexHeight;
    public float apexTime;
    public float gravity;
    public float jumpVelocity;
    private float velocity;
    public bool isJumping;
    public float time;

    public enum FacingDirection
    {
        left, right
    }

    PlayerController.FacingDirection lastDirection;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        Vector2 playerInput = new Vector2(Input.GetAxis("Horizontal"), 0);
        Vector2 playerJump = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        time = Time.deltaTime;

        MovementUpdate(playerInput,playerJump);

    }

    private void MovementUpdate(Vector2 playerInput , Vector2 playerJump)
    {
        apexTime = apexTime * time;

        rb.velocity = playerInput * maxSpeed;
        rb.AddForce(playerInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gravity = -2 * apexHeight / (Mathf.Pow(apexTime ,2 ));
            jumpVelocity = 2 * apexHeight / apexTime;
            velocity = gravity * time + jumpVelocity;

            rb.velocity = new Vector2(rb.velocity.x , rb.velocity.y) * maxSpeed * velocity;


            isJumping = true;
        }

        // I did this early it is supposed to be part of task 1
        if(isJumping == true)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 1;
        }

    }

    public bool IsWalking()
    {
        return (Input.GetAxis("Horizontal") != 0);


    }

    public bool IsGrounded()
    {
        return !(Physics2D.Raycast(transform.position, Vector2.down, 0.75f, LayerMask.GetMask("ground")));

    }

    public FacingDirection GetFacingDirection()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            lastDirection = FacingDirection.right;
            return FacingDirection.right;
        }
       
        if (Input.GetAxis("Horizontal") < 0)
        {
            lastDirection = FacingDirection.left;
            return FacingDirection.left;
        }
        return lastDirection;
    }

}
