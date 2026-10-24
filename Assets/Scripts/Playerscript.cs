using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Automatically adds a Rigidbody2D if one is missing
[RequireComponent(typeof(Rigidbody2D))]
public class Playerscript : MonoBehaviour
{
    // Event fired when the player dies
    public static event Action Death;

    // Displays the player's health on screen
    public TextMeshPro Healthtext;


    // Health

    [Header("Health")]
    public int Maximum_health = 100;
    public int health;

    // Prevents movement and actions after death
    public bool IsDead { get; private set; }

    // Sprites

    [Header("Components")]
    public SpriteRenderer SR;
    public SpriteRenderer GunSR;



    // Movement Section

    [Header("Movement")]
    public float maxSpeed = 15f;
    public float speed;

    // Sprint speed multiplier
    public float sprintMultiplier = 2f;


    // Jumping Section

    [Header("Jumping")]
    public float maxJumpingForce = 17.5f;
    public float jumpingForce;


    // Cloud Bounce Section

    [Header("Cloud Bounce")]
    public float CloudFloatingForce;
    public float cloudFloatingDuation;
    private bool isFloating;
    private float floatTimer;
    private Vector2 targetPosition;


    // Gun
    [Header("Pistol")]
    public GameObject bulletPrefab;
    public Transform gun;
    public float bulletSpeed = 40f;

    // Physics private components
    private Rigidbody2D rig;

    // Double jump system
    private int remainingJumps;
    private const int maxJumps = 2;

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();

        // Shows player stats
        health = Maximum_health;
        speed = maxSpeed;
        jumpingForce = maxJumpingForce;

        // Allow double jumps
        remainingJumps = maxJumps;

        // Player starts off alive
        IsDead = false;

        // Automatically find SpriteRenderer if it is not assigned
        if (SR == null)
        {
            SR = GetComponent<SpriteRenderer>();
        }

        UpdateHealth();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Helps Bounce off clouds
        if (collision.gameObject.CompareTag("Cloud"))
        {
            Debug.Log("Touched Cloud!");
            isFloating = true;
            floatTimer = cloudFloatingDuation;
                
            remainingJumps = maxJumps;
            return;
        }

        // Reset jumps when standing on ground/platforms
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                remainingJumps = maxJumps;
                break;
            }
        }
    }

    // This is called when the player takes damage
    public void Damage(int damage)
    {
        if (IsDead)
            return;

        health = Mathf.Max(0, health - damage);

        UpdateHealth();

        Debug.Log("Health: "+ health);

        // Handles how the player dies 
        if (health == 0)
        {
            IsDead = true;

            if (SR != null)
            {
                SR.color = Color.red;
            }

            Death?.Invoke();

            Debug.Log("Player Died");

            SceneManager.LoadScene("Game Over");
        }
    }

    private void Update()
    {
        if (isFloating)
        {
            floatTimer -= Time.deltaTime;

            rig.linearVelocity = new Vector2(
                rig.linearVelocity.x,
                CloudFloatingForce);
            if (floatTimer <= 0f)
            {
                isFloating = false;
            }
        }
        
        
        if (IsDead)
            return;

        if (Keyboard.current == null)
            return;

        float moveX = 0f;

        // Move left and flip sprites connected to player
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            moveX -= 1f;

            SR.flipX = true;
            GunSR.flipX = true;
        }

        // Move right and flip sprites connected to the player
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            moveX += 1f;

            SR.flipX = false;
            GunSR.flipX = false;
        }

        // Apply sprint multiplier when sprinting
        float currentSpeed = speed;

        if (Keyboard.current.shiftKey.isPressed)
        {
            currentSpeed *= sprintMultiplier;
        }

        // Controls the movement speed
        rig.linearVelocity = new Vector2(
            moveX * currentSpeed,
            rig.linearVelocity.y
        );

        // Jump / double jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            remainingJumps > 0)
        {
            rig.linearVelocity = new Vector2(
                rig.linearVelocity.x,
                jumpingForce
            );

            remainingJumps--;
        }

        // Shoot when S is pressed
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    // Updates the health text UI
    public void UpdateHealth()
    {
        if (Healthtext != null)
        {
            Healthtext.text = "Health: " + health;
        }
    }

    // Creates a bullet and fires it to it's target
    //Fine Damage limit for zombies
    private void Shoot()
    {
        if (bulletPrefab == null || gun == null)
            return;
        //Shows which way to help shoot the bullets depending on the way the characher is facing//

        float direction = SR.flipX ? -1f : 1f;

        // Spawns in front of the gun//
        Vector3 spawnPosition = gun.position + new Vector3(direction * 0.5f, 0f, 0f);
        //Creates a new bullet after the first one//
        GameObject bullet = Instantiate(
            bulletPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            //Horizontal Velocity/speed //
            bulletRb.linearVelocity = new Vector2(
                direction * bulletSpeed,
                0f);// Means no vertical movement//
            //Add vertical movement later//
        }

        // flips the bullet sprite dependeing on player x axis
        SpriteRenderer bulletSprite = bullet.GetComponent<SpriteRenderer>();

        if (bulletSprite != null)
        {
            //Helps the bullet flip visually//
            bulletSprite.flipX = SR.flipX;
        }
    }
}