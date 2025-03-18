using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private LevelLoader levelLoader;

    public float moveSpeed = 5f; // Hareket hızı
    public float jumpForce = 7f; // Zıplama gücü
    private Rigidbody2D rb;
    private bool isGrounded;

    // Dokunmatik Kontroller
    private bool moveLeft = false;
    private bool moveRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        levelLoader = FindObjectOfType<LevelLoader>(); // LevelLoader scriptini bul
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float moveInput = 0f; // Hareket Girdisi

        // **Dokunmatik Kontroller** (Öncelikli)
        if (moveLeft) moveInput = -1;
        if (moveRight) moveInput = 1;

        // **Klavye Kontrolleri** (Eğer dokunmatik kullanılmıyorsa)
        if (!moveLeft && !moveRight)
        {
            moveInput = Input.GetAxisRaw("Horizontal"); // "A" ve "D" veya Sol-Sağ yön tuşları
        }

        // Hareketi uygula (Yalnızca sağa veya sola hareket eder, zıplamadan etkilenmez)
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // Dokunmatik Kontroller (Butona Basınca Çalışır)
    public void MoveLeftDown() => moveLeft = true;
    public void MoveLeftUp() => moveLeft = false;

    public void MoveRightDown() => moveRight = true;
    public void MoveRightUp() => moveRight = false;

    // Yerde olup olmadığını kontrol etme
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EndPoint")) // Eğer oyuncu EndPoint'e girdiyse
        {
            levelLoader.LoadNextLevel();
        }
    }
}
