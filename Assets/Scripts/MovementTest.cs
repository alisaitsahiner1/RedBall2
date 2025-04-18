using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float speed =5f;
    public float maxSpeed = 5f; // Maksimum izin verilen yatay hız
    public float jumpPower;
    private Rigidbody2D rb;
    private LevelLoader levelLoader;
    private float horizontalInput = 0f; // UI ve klavye girdileri birleşsin diye


    [Header("Zemin Ayarları")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public bool isGrounded;

    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        levelLoader = FindObjectOfType<LevelLoader>(); // LevelLoader scriptini bul
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        if (x != 0)
    {
        horizontalInput = x; // Klavye girdisi varsa UI'yı geçersiz kıl
    }

        float y = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(horizontalInput,0);
        Run(direction);


        CheckGrounded();

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump(Vector2.up);
        }
    }

    public void Run(Vector2 dir)
    {
        float currentSpeed = rb.velocity.x;

        // Hareket yönüne göre maksimum hızı kontrol et
        if ((dir.x > 0 && currentSpeed < maxSpeed) || (dir.x < 0 && currentSpeed > -maxSpeed))
        {
            rb.AddForce(new Vector2(dir.x * speed, 0));
        }
    }


    public void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x , 0);
        rb.velocity += dir*jumpPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("EndPoint")) // Eğer oyuncu EndPoint'e girdiyse
        {
            levelLoader.LoadNextLevel();
        }
    }

    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void CheckGrounded()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Ground"))
            {
                isGrounded = true;
                break;
            }
        }
    }

    public void MoveRight()
    {
        horizontalInput = 1f;
    }

    public void MoveLeft()
    {
        horizontalInput = -1f;
    }

    public void StopMoving()
    {
        horizontalInput = 0f;
    }

    public void UIMoveJump()
    {   
        if (isGrounded)
            Jump(Vector2.up);
    }


}
