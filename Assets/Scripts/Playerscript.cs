using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Automatically adds a Rigidbody2D if one is missing
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]

public class Playerscript : MonoBehaviour
{
    // Event fired when the player dies
    public static event Action Death;

    // Displays the player's Health on screen
    public TextMeshPro HealthText;
    public ParticleSystem JumpDust;
    public ParticleSystem BulletDust;

    public AudioSource AudioSource;
    public AudioClip JumpSound;
    public AudioClip Gunshot; 


    // Health
    [Header("Health")]
    public int Maximum_Health = 100;
    public int Health;

    // Prevents movement and actions after death
    public bool IsDead { get; private set; }

    // Sprites
    [Header("Components")]
    public SpriteRenderer SR;
    public SpriteRenderer GunSR;

    // Movement Section
    [Header("Movement")]
    public float MaxSpeed = 15f;
    public float Speed;

    // Sprint Speed multiplier
    public float SprintMultiplier = 2f;


    // Jumping Section

    [Header("Jumping")]
    public float MaxJumpingForce = 50f;
    public float JumpingForce;
    private Vector3 normalScale;
    // Cloud Bounce Section

    [Header("Cloud Bounce")]
    public float CloudFloatingForce; // Vertical velocity while floating midair
    public float CloudFloatingDuation; // How long the player floats for
    private bool IsFloating; // Sees if the player is floating
    private float FloatTimer;// Timer for floating


    // Gun
    [Header("Pistol")]
    public GameObject bulletPrefab;
    public Transform gun;
    public Transform MuzzleRight;
    public Transform MuzzleLeft;
    public float bulletSpeed = 40f;
    public float damage = 5f;
    private float GunVisibilityTime = 2f;
    private float GunFadingSpeed = 3f;
    private float GunTimer;
    private Color GunColor;
    private Vector3 gunStartposition;
    public float Timer = 3;



    // Physics private components
    private Rigidbody2D Rigidbody;

    // Double jump system
    private int RemainingJumps;
    private const int MaxJumps = 2;

    public void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        // Shows player stats
        Health = Maximum_Health;
        Speed = MaxSpeed;
        JumpingForce = MaxJumpingForce;
        normalScale = SR.transform.localScale;
        gunStartposition = gun.localPosition;
        // Allow double jumps
        RemainingJumps = MaxJumps;

        // Player starts off alive
        IsDead = false;

        // Automatically find SpriteRenderer if it is not assigned
        if (SR == null)
        {
            SR = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        }
        if (GunSR != null)
        {
            GunColor = GunSR.color;
            //Starts off hidden until clicking the f key
            GunColor.a = 0f;
            GunSR.color = GunColor;
        }
        if (AudioSource == null)
        {
            AudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        }

        UpdateHealth();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Helps Bounce off clouds
        if (collision.gameObject.CompareTag("Cloud"))
        {
            Debug.Log("Touched Cloud!");
            IsFloating = true;
            FloatTimer = CloudFloatingDuation;
                
            RemainingJumps = MaxJumps;
            return;
        }

        // Reset jumps when standing on ground/platforms
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                RemainingJumps = MaxJumps;
                break;
            }
        }
    }

    // This is called when the player takes damage
    public void Damage(int damage)
    {
        if (IsDead)
            return;

        Health = Mathf.Max(0, Health - damage);

        UpdateHealth();

        Debug.Log("Health: "+ Health);

        // Handles how the player dies 
        if (Health == 0)
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
        if (IsFloating)
        {
            FloatTimer -= Time.deltaTime;

            Rigidbody.linearVelocity = new Vector2(
                Rigidbody.linearVelocity.x,
                CloudFloatingForce);
            if (FloatTimer <= 0f)
            {
                IsFloating = false;
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

            gun.localPosition = new Vector3(-Mathf.Abs(gunStartposition.x),gunStartposition.y, gunStartposition.z);
        }

        // Move right and flip sprites connected to the player
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            moveX += 1f;

            SR.flipX = false;
            GunSR.flipX = false;

            gun.localPosition = new Vector3(Mathf.Abs(gunStartposition.x), gunStartposition.y, gunStartposition.z);
        }

        // Apply sprint multiplier when sprinting
        float currentSpeed = Speed;

        if (Keyboard.current.shiftKey.isPressed)
        {
            currentSpeed *= SprintMultiplier;
        }

        // Controls the movement Speed
        Rigidbody.linearVelocity = new Vector2(
            moveX * currentSpeed,
            Rigidbody.linearVelocity.y
        );

        // Jump / double jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            RemainingJumps > 0)
        {
            Rigidbody.linearVelocity = new Vector2(
                Rigidbody.linearVelocity.x,
                JumpingForce
            );
            SR.transform.localScale = new Vector3(normalScale.x * 0.95f, normalScale.y * 1.05f, normalScale.z);
            RemainingJumps--;

            if (JumpDust != null)
            {
                JumpDust.Play();
            }
            if (AudioSource != null && JumpSound != null)
            {
                AudioSource.PlayOneShot(JumpSound);
            }
        }

        // Shoot when F is pressed
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Timer -= Time.deltaTime;
            Shoot();
            BulletDust.Play();
            Timer = 3;
            if (AudioSource != null && Gunshot != null)
            {
                AudioSource.PlayOneShot(Gunshot);
            }
        }
        SR.transform.localScale = Vector3.Lerp(SR.transform.localScale, normalScale,10f * Time.deltaTime);
    }
    private void GunVisibility()
    {
        if (GunSR == null)
            return;
        if (GunTimer > 0f)
        {
            GunTimer -= Time.deltaTime;
            //Fades in
            GunColor.a = Mathf.Lerp(GunColor.a, 1f, GunFadingSpeed * Time.deltaTime);
        }
        else
        {
            GunColor.a = Mathf.Lerp(GunColor.a, 0f, GunFadingSpeed * Time.deltaTime);
            GunSR.color = GunColor;
        }
    }

    // Updates the Health text UI
    public void UpdateHealth()
    {
        if (HealthText != null)
        {
            HealthText.text = "Health: " + Health;
        }
    }

    // Creates a bullet and fires to it's target
    //Find Damage limit for zombies
    private void Shoot()
    {
        GunTimer = GunVisibilityTime;
        GunColor.a = 1f;
        GunSR.color = GunColor;
        if (bulletPrefab == null || gun == null)
            return;
        //Shows which way to help shoot the bullets depending on the way the characher is facing//
        float direction = SR.flipX ? -1f : 1f;

        BulletDust.transform.position = SR.flipX? MuzzleLeft.position : MuzzleRight.position;
        BulletDust.Play();


        // Spawns in front of the gun//
        Vector3 spawnPosition = SR.flipX ? MuzzleLeft.position : MuzzleRight.position;
        //Creates a new bullet after the first one//
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // Velocity Section
        if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
        {
            //Horizontal Velocity/Speed //
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