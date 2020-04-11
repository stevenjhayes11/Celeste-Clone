using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float jumpFactor;
    [SerializeField] float runFactor;
    // Start is called before the first frame update
    Rigidbody2D playerBody;
    float stamina;
    bool onGround;
    bool holdingLeft;
    bool holdingRight;
    bool drainStamina;
    bool wallIsRight;
    [SerializeField] float staminaDrainRate;
    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        onGround = false;
        stamina = 100;
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
        }
        else if (collision.gameObject.tag == "Wall")
        {
            drainStamina = true;
            float wallX = collision.transform.position.x;
            float playerX = playerBody.transform.position.x;
            wallIsRight = wallX > playerX;
            AttemptWallHang();
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
            drainStamina = false;
        }
    }

    void AttemptWallHang()
    {
        if(stamina > 0)
        {
            DrainStamina();
            Hang();
        }
        else
        {
            print("NO STAMINA");
        }
    }
    void Hang()
    {
        if(wallIsRight && holdingRight)
        {
            playerBody.velocity = new Vector2(0f, 0f);
        }
        if (!wallIsRight && holdingLeft)
        {
            playerBody.velocity = new Vector2(0f, 0f);
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
    void Jump()
    {
        if(onGround)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpFactor);
            onGround = false;
        }
        
    }

    void StopVertical()
    {
        playerBody.velocity = new Vector2(playerBody.velocity.x, 0f);
    }
    private void StopHorizontal()
    {
        playerBody.velocity = new Vector2(0f, playerBody.velocity.y);
    }
    
}

