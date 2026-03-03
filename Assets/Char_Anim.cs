using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Char_Anim : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public float mult = 2f;
    public float dur = 4f;

    private bool moving = false;
    private bool facingLeft = false;

    public AudioSource sfxSource;
    public AudioClip jumpClip;
    public AudioClip powerupClip;
    public AudioClip coinClip;
    public AudioClip enemyKillClip;
    public AudioClip levelFinishClip;
    public int health = 3;
    public Text healthDisplay;
    Vector3 spawnPoint; // This will store the starting position

    Rigidbody2D rb;
    Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spawnPoint = transform.position;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAnimation();

        if (transform.position.y < -10f) 
        {
            TakeDamage(1); 
        }
    }

    void HandleMovement()
    {
        float horizontal = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            horizontal = -1f;
            facingLeft = true;
            moving = true;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            horizontal = 1f;
            facingLeft = false;
            moving = true;
        } else
        {
            moving = false;
        }

            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (jumpClip != null)
            {
                sfxSource.PlayOneShot(jumpClip);
            }
                
        }

        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("powerup"))
        {
            StartCoroutine(Pickup(other));
        }
    }

    IEnumerator Pickup(Collider2D powerup)
    {
        Destroy(powerup.gameObject);
        moveSpeed *= mult;
        yield return new WaitForSeconds(dur);
        moveSpeed /= mult;
    }

    public void PlayCoinSFX()
    {
        if (coinClip != null)
        {
            sfxSource.PlayOneShot(coinClip);
        }
    }

    public void PlayEnemyKillSFX()
    {
        sfxSource.PlayOneShot(enemyKillClip);
    }

    public void PlayLevelFinishSfx()
    {
        sfxSource.PlayOneShot(levelFinishClip);
    }

    void HandleAnimation()
    {
        animator.SetBool("Moving", moving);
        animator.SetBool("FacingLeft", facingLeft);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            // Restarts the level if out of hearts
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        else
        {
            // Move player back to start and reset their speed so they don't keep falling
            transform.position = spawnPoint;
            rb.linearVelocity = Vector2.zero;
        }
        if (healthDisplay != null) {
            healthDisplay.text = "Hearts: " + health;
        }
    }
    
}

