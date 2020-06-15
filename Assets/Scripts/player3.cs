using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player3 : MonoBehaviour
{
    [SerializeField] float runSpeed;
    [SerializeField] float accelerationSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float wallJumpWaitTime;

   playerState currentState;
    Rigidbody2D playerBody;
    GameObject recentWallCollision;
    bool playerCanMove;
    /*
     * playerState is for keeping track of what the player is touching
     * so that appropriate actions can be taken on input
     */
    enum playerState
    {
        inAir,
        onGround,
        onWall
    }
    // Start is called before the first frame update
    void Start()
    {
        playerCanMove = true;
        currentState = playerState.onGround;
        playerBody = GetComponent<Rigidbody2D>();
        recentWallCollision = GetComponent<GameObject>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If collision object is a new wall, set playerState to onWall
        if (collision.gameObject.tag == "Wall")
        {
            if (!GameObject.ReferenceEquals(recentWallCollision, collision.gameObject))
                currentState = playerState.onWall;
        }
        //if collision is ground set playerState on ground
        if (collision.gameObject.tag == "Ground")
        {
            currentState = playerState.onGround;

        }
        //store recent collision for wall jump method
        recentWallCollision = collision.gameObject;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        currentState = playerState.inAir;
    }

    // Update is called once per frame
    void Update()
    {
        //if no key is pressed, revert gravity to normal
        if(!Input.anyKey)
        {
            playerBody.gravityScale = 1f;
        }
        if (playerCanMove)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                bool left = Input.GetKey(KeyCode.A);
                MoveHorizontal(left);
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                StopHorizontal();
            }
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
            if(Input.GetKeyUp(KeyCode.Space))
            {
                if(playerBody.velocity.y > 0)
                    StopVertical();
            }
        }
    }
    /*
     * Tries to move the player horizontally
     */
    void MoveHorizontal(bool left)
    {
        bool tryWallHang = false;
        //if the player is on the wall and moving towards the wall they are on
        //attempt to hang
        if(currentState == playerState.onWall)
        {
            float wallX = recentWallCollision.transform.position.x;
            float playerX = playerBody.transform.position.x;
            bool wallToLeft = wallX < playerX;
            if((wallToLeft && left) || (!wallToLeft && !left))
            {
                tryWallHang = true;
            }
        }
        if(tryWallHang)
        {
            WallHang();
        }
        //otherwise move normally and revert gravity
        else
        {
            playerBody.gravityScale = 1f;
            if (left)
                MoveLeft();
            else
                MoveRight();
        }
    }
    /*
     * Freeze player on wall
     */
    void WallHang()
    {
        playerBody.velocity = new Vector2(0.0f, 0.0f);
        playerBody.gravityScale = 0.0f;
    }
    void MoveLeft()
    {
        
        if (playerBody.velocity.x > -runSpeed)
        {
            playerBody.velocity = playerBody.velocity + new Vector2(-(accelerationSpeed) * runSpeed, 0);
        }
    }
    void MoveRight()
    {
        if (playerBody.velocity.x < runSpeed)
        {
            playerBody.velocity = playerBody.velocity + new Vector2(accelerationSpeed * runSpeed, 0);
        }
    }
    private void StopHorizontal()
    {
        playerBody.velocity = new Vector2(0f, playerBody.velocity.y);
    }

    void Jump()
    {
        if (currentState == playerState.onGround)
        {
            GroundJump();
        }
        else if (currentState == playerState.onWall)
        {
            WallJump();
        }
        currentState = playerState.inAir;
    }
    void StopVertical()
    {
        playerBody.velocity = new Vector2(playerBody.velocity.x, 0f);
    }
    void GroundJump()
    {
        playerBody.velocity = new Vector2(playerBody.velocity.x, jumpSpeed);
    }
    /*
     * wall jumping takes control from the player for a short period of time
     * so they cannot come right back to the wall they jump off
     */
    void WallJump()
    {
        playerCanMove = false;
        float wallX = recentWallCollision.transform.position.x;
        float playerX = playerBody.transform.position.x;
        bool wallToLeft = wallX < playerX;
        if (wallToLeft)
            playerBody.velocity = new Vector2(jumpSpeed / 2f, jumpSpeed / 2f);
        else
            playerBody.velocity = new Vector2(-jumpSpeed / 2f, jumpSpeed / 2f);
        StartCoroutine(WallJumpWait());
    }
    /*
     * For waiting to return control to player after a wall jump
     */
    IEnumerator WallJumpWait()
    {
        yield return new WaitForSeconds(wallJumpWaitTime);
        playerCanMove = true;
    }
}
