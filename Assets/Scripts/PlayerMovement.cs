using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float jumpFactor;
    [SerializeField] float runFactor;
    [SerializeField] float wallJumpXVelocity;
    [SerializeField] float staminaDrainRate;
    // Start is called before the first frame update
    GameObject recentWallCollision;
    Rigidbody2D playerBody;
    float stamina;
    bool onGround;
    bool holdingLeft;
    bool holdingRight;
    bool drainStamina;
    bool wallIsRight;
    bool hanging;
    
    void Start()
    {
        recentWallCollision = GetComponent<GameObject>();
        playerBody = GetComponent<Rigidbody2D>();
        onGround = false;
        stamina = 100;
        hanging = false;
        holdingLeft = false;
        holdingRight = false;
    }

    // Update is called once per frame
    
    private void FixedUpdate()
    {
       if(Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
    }

    void Update()
    {
        if(drainStamina)
        {
            AttemptWallHang();
        }
        if (Input.GetKey(KeyCode.A))
        {
            holdingLeft = true;
        }
        else
        {
            holdingLeft = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            holdingRight = true;
        }
        else
        {
            holdingRight = false;
        }
        if (Input.GetKey(KeyCode.L))
        {
            LoadLevel();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        if(Input.GetKeyUp(KeyCode.Space) && (playerBody.velocity.y > 0))
        {
            StopVertical();
        }
        if ((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)))
        {
            StopHorizontal();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && playerBody.velocity.y <= Mathf.Epsilon)
        {
            onGround = true;
            recentWallCollision = GetComponent<GameObject>();
        }
        else if (collision.gameObject.tag == "Wall" && recentWallCollision.GetInstanceID() != collision.gameObject.GetInstanceID())
        {
            print("same wall not collided");
            recentWallCollision = collision.gameObject;
            drainStamina = true;
            float wallX = collision.transform.position.x;
            float playerX = playerBody.transform.position.x;
            wallIsRight = wallX > playerX;
            AttemptWallHang();
        }
        else if (collision.gameObject.tag == "Wall" && recentWallCollision.GetInstanceID() == collision.gameObject.GetInstanceID())
        {
            print("same wall Collided");
        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = false;
        }
        if(collision.gameObject.tag == "Wall")
        { 
            hanging = false;
            drainStamina = false;
            playerBody.gravityScale = 1.0f;
        }
    }

    void AttemptWallHang()
    {
        if(stamina > 0 && drainStamina)
        {
            hanging = true;
            DrainStamina();
            Hang();
        }
        else if (stamina <=0)
        {
            print("NO STAMINA");
            drainStamina = false;
            playerBody.gravityScale = 1.0f;
        }
        else
        {
            print("voluntary seperation");
        }
    }
    void Hang()
    {
        if ((wallIsRight && holdingRight) || (!wallIsRight && holdingLeft))
        {
            playerBody.velocity = new Vector2(0.0f, 0.0f);
            playerBody.gravityScale = 0.0f;
        }
        else
        {
            drainStamina = false;
        }
    }
    void DrainStamina()
    {
        stamina = stamina - (Time.deltaTime * (staminaDrainRate / 500f));
    }






    void LoadLevel()
    {
        SceneManager.LoadScene(GameManager.instance.GetLevel() + 1);
    }
    void MoveLeft()
    {
        playerBody.velocity = new Vector2(-runFactor, playerBody.velocity.y);
    }
    void MoveRight()
    {
        playerBody.velocity = new Vector2(runFactor, playerBody.velocity.y);
    }
    void StopVertical()
    {
        playerBody.velocity = new Vector2(playerBody.velocity.x, 0f);
    }
    private void StopHorizontal()
    {
        playerBody.velocity = new Vector2(0f, playerBody.velocity.y);
    }
    void Jump()
    {
        if (onGround)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpFactor);
            onGround = false;
        }
    }
}

