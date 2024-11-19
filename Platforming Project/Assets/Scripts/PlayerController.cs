using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float maxSpeed = 6;
    Vector2 playerInput;

    public enum FacingDirection
    {
        left, right
    }

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
        Vector2 playerInput = new Vector2(Input.GetAxis("Horizontal") , 0);
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        rb.velocity = playerInput * maxSpeed;
        rb.AddForce(playerInput);
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
            return FacingDirection.right;
        }
       
        if (Input.GetAxis("Horizontal") < 0)
        {
            return FacingDirection.left;
        }
        return FacingDirection.right;
    }
}
