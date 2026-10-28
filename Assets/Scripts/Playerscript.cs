using System;
using TMPro;
using Unity.XR.OpenVR;
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
    public ParticleSystem jumpDust;
    public ParticleSystem bulletDust;

    public AudioSource audioSource;
    public AudioClip jumpSound;


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
    public float maxJumpingForce = 50f;
    public float jumpingForce;
    private Vector3 normalScale;
    // Cloud Bounce Section

    [Header("Cloud Bounce")]
    public float CloudFloatingForce; // Vertical velocity while floating midair
    public float cloudFloatingDuation; // How long the player floats for
    private bool isFloating; // Sees if the player is floating
    private float floatTimer;// Timer for floating


    // Gun
    [Header("Pistol")]
    public GameObject bulletPrefab;
    public Transform gun;
    public float bulletSpeed = 40f;
    public float damage = 5f;
    private float gunVisibilityTime = 2f;
    private float gunFadingSpeed = 3f;
    private float guntimer;
    private Color gunColor;

    // Physics private components
    private Rigidbody2D rig;

    // Double jump system
    private int remainingJumps;
    private const int maxJumps = 2;

    public void Start()
    {
        rig = GetComponent<Rigidbody2D>();

        // Shows player stats
        health = Maximum_health;
        speed = maxSpeed;
        jumpingForce = maxJumpingForce;
        normalScale = SR.transform.localScale;

        // Allow double jumps
        remainingJumps = maxJumps;

        // Player starts off alive
        IsDead = false;

        // Automatically find SpriteRenderer if it is not assigned
        if (SR == null)
        {
            SR = GetComponent<SpriteRenderer>();
        }
        if (GunSR != null)
        {
            gunColor = GunSR.color;
            //Starts off hidden until clicking the f key
            gunColor.a = 0f;
            GunSR.color = gunColor;
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

    public void Update()
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
        GunVisibility();
        
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
            SR.transform.localScale = new Vector3(normalScale.x * 0.95f, normalScale.y * 1.05f, normalScale.z);
            remainingJumps--;

            if (jumpDust != null)
            {
                jumpDust.Play();
            }
            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }

        // Shoot when F is pressed
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Shoot();
            bulletDust.Play();
        }
        ;
        SR.transform.localScale = Vector3.Lerp(SR.transform.localScale, normalScale,10f * Time.deltaTime);
    }
    private void GunVisibility()
    {
        if (GunSR == null)
            return;
        if (guntimer > 0f)
        {
            guntimer -= Time.deltaTime;
            //Fades in
            gunColor.a = Mathf.Lerp(gunColor.a, 1f, gunFadingSpeed * Time.deltaTime);
        }
        else
        {
            gunColor.a = Mathf.Lerp(gunColor.a, 0f, gunFadingSpeed * Time.deltaTime);
            GunSR.color = gunColor;
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

    // Creates a bullet and fires to it's target
    //Find Damage limit for zombies
    private void Shoot()
    {
        guntimer = gunVisibilityTime;
        gunColor.a = 1f;
        GunSR.color = gunColor;
        if (bulletPrefab == null || gun == null)
            return;
        //Shows which way to help shoot the bullets depending on the way the characher is facing//

        float direction = SR.flipX ? -1f : 1f;

        // Spawns in front of the gun//
        Vector3 spawnPosition = gun.position + new Vector3(direction * 0.5f, 0f,0f);
        //Creates a new bullet after the first one//
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity
        );

        
        if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
        {
            //Horizontal Velocity/speed //
            bulletRb.linearVelocity = new Vector2(direction * bulletSpeed, 0f);// 0f Means no vertical movement//
            //Add vertical movement later//
        }

        // flips the bullet sprite dependeing on player x axis
        
        if (bullet.TryGetComponent<SpriteRenderer>(out var bulletSprite))
        {
            //Helps the bullet flip vertically visually//
            bulletSprite.flipX = SR.flipX;
        }
        if (bullet.TryGetComponent<Bullet>(out var bulletScript))
        {
            bulletScript.damage = damage;
        }
    }
}