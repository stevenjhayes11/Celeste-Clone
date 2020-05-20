using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float jumpFactor;
    [SerializeField] float runFactor;

    Rigidbody2D playerBody;

    bool onWall;
    bool onGround;
    int jumpCharges;
    float stamina;
    bool wallJumping;
    GameObject recentWallCollision;
    // Start is called before the first frame update
    void Start()
    {
        recentWallCollision = GetComponent<GameObject>();
        stamina = 100;
        playerBody = GetComponent<Rigidbody2D>();
        jumpCharges = 0;
        onWall = false;
        wallJumping = false;
        onGround = false;
    }

    private void FixedUpdate()
    {
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && (Input.GetKey(KeyCode.Space)) && jumpCharges > 0)
        {
            Jump();
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && !wallJumping)
        {
            bool left = Input.GetKey(KeyCode.A);
            MoveHorizontal(left);
        }
        else if(wallJumping)
        {
            print("no jumping");
        }
        
        
    }
    // Update is called once per frame
    void Update()
    {
        if((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) && !wallJumping)
        {
            StopHorizontal();
        }
        if (Input.GetKeyUp(KeyCode.Space) && (playerBody.velocity.y > 0) && !wallJumping)
        {
            StopVertical();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            onGround = true;
            recentWallCollision = GetComponent<GameObject>();
            if (jumpCharges == 0)
            {
                jumpCharges++;
            }
        }
        if (collision.gameObject.tag == "Wall" && !GameObject.ReferenceEquals(recentWallCollision, collision.gameObject))
        {
            recentWallCollision = collision.gameObject;
            onWall = true;
            if (jumpCharges == 0)
            {
                jumpCharges++;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            onWall = false;
            playerBody.gravityScale = 1f;
        }
        if (collision.gameObject.tag =="Ground")
        {
            onGround = false;
        }
    }
    void Jump()
    {
        if (jumpCharges > 0 && !onWall)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpFactor);
            jumpCharges--;
        }
        else if(jumpCharges > 0 && onWall)
        {
            float wallX = recentWallCollision.transform.position.x;
            float playerX = playerBody.transform.position.x;
            bool wallRight = wallX > playerX;
            float halfJump = jumpFactor / 2f;
            StartCoroutine(WallJumpWait());
            if(wallRight)
            {
                playerBody.velocity = new Vector2(-(halfJump), halfJump);
            }
            else
            {
                playerBody.velocity = new Vector2((halfJump), halfJump);
            }
            jumpCharges--;
        }
    }
    IEnumerator WallJumpWait()
    {
        wallJumping = true;
        print("pre " + wallJumping);
        yield return new WaitForSeconds(1f);
        wallJumping = false;
        print("post " + wallJumping);
    }
    void StopVertical()
    {
        playerBody.velocity = new Vector2(playerBody.velocity.x, 0f);
    }
    void Hang()
    {
        playerBody.velocity = new Vector2(0.0f, 0.0f);
        playerBody.gravityScale = 0.0f;
    }
    private void MoveHorizontal(bool left)
    {
        if (onWall)
        {
            float wallX = recentWallCollision.transform.position.x;
            float playerX = playerBody.transform.position.x;
            bool wallRight = wallX > playerX;
            if ((wallRight && !left) || (!wallRight && left))
            {
                Hang();
            }
        }
        if(left)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }
    }
    private void StopHorizontal()
    {
         playerBody.velocity = new Vector2(0f, playerBody.velocity.y);
    }
    void MoveLeft()
    {
        if (onGround && playerBody.velocity.x > -runFactor)
            playerBody.velocity = playerBody.velocity + new Vector2(-runFactor, 0f);
        else if (playerBody.velocity.x > -runFactor/2f)
            playerBody.velocity = playerBody.velocity + new Vector2(-runFactor/2f, 0f);
    }
    void MoveRight()
    {
        if (onGround && playerBody.velocity.x < runFactor)
            playerBody.velocity = playerBody.velocity + new Vector2(runFactor, 0f);
        else if (playerBody.velocity.x < runFactor / 2f)
            playerBody.velocity = playerBody.velocity + new Vector2(runFactor / 2f, 0f);
    }
}
