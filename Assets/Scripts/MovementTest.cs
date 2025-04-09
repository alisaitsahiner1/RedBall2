using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float speed =5f;
    public float jumpPower;
    private Rigidbody2D rb;
    private LevelLoader levelLoader;
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        levelLoader = FindObjectOfType<LevelLoader>(); // LevelLoader scriptini bul
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(x,y);
        Run(direction);

        if(Input.GetButtonDown("Jump"))
        {
            Jump(Vector2.up);
        }
    }

    public void Run (Vector2 dir)
    {
        rb.AddForce(new Vector2(dir.x*speed , 0));
    }

    public void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x , 0);
        rb.velocity += dir*jumpPower;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EndPoint")) // Eğer oyuncu EndPoint'e girdiyse
        {
            levelLoader.LoadNextLevel();
        }
    }

}
