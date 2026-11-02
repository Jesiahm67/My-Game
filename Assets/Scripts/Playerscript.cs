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
    public AudioSource AudioSource;
    public TextMeshPro ZombieKillText;
 
    // Health Part //
    [Header("Health")]
    public int Maximum_Health = 100;
    public int Health;
    public TextMeshPro HealthText;

    [Header("Time")]
    public float Gaming_Time = 180f;
    private bool Times_Up = false;
    public TextMeshPro Timer;
    [Header("Score")]
    public int ZombiesKilled = 0;
    // Kills goals to unlock goals
    [Header("Kill Goals")]
    public int SecretExitKills = 25;
    public int LevelExitKills = 55;
    public GameObject SecretExit;
    public GameObject LevelExit;
    private bool secretExitUnlocked;
    private bool LevelExitUnlocked;


    // Prevents movement and actions after death
    public bool IsDead { get; private set; }
    // Sprites Part //
    [Header("Components")]
    public SpriteRenderer SR;
    public SpriteRenderer GunSR;

    // Movement Section
    [Header("Movement")]
    public float MaxSpeed = 30f;
    public float Speed;
    // Sprint Speed multiplier
    public float SprintMultiplier = 2f;


    // Jumping Section

    [Header("Jumping")]
    public float MaxJumpingForce = 50f;
    public float JumpingForce;
    public ParticleSystem JumpDust;
    public AudioClip JumpSound;

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
    public ParticleSystem BulletDust;
    public AudioClip Gunshot;
    public Transform gun;
    public Transform MuzzleRight;
    public Transform MuzzleLeft;
    public Transform MuzzleDown;
    public Transform MuzzleUp;
    public float bulletSpeed = 40f;
    public float damage = 15f;
    private float GunVisibilityTime = 2f;
    private float GunFadingSpeed = 3f;
    private float GunTimer;
    private Color GunColor;
    private Vector3 gunStartposition;
    private enum ShootDirection {Left, Right, Up, Down}
    private ShootDirection currentDirection = ShootDirection.Right;



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

        string currentScene = SceneManager.GetActiveScene().name;    
        // Kills for secret exit and normal exit to unlock
        switch (currentScene)
        {
            case "Level 1":
                ZombiesKilled = GameData.Level1Kills;
                SecretExitKills = 30;
                LevelExitKills = 55;
                break;
            case "Level 2":
                ZombiesKilled = GameData.Level2Kills;
                SecretExitKills = 60;
                LevelExitKills = 65;
                break;
            case "Level 3":
                ZombiesKilled = GameData.Level3Kills;
                SecretExitKills = 70;
                LevelExitKills = 75;
                break;
            case "Secret Level":
                ZombiesKilled = GameData.SecretLevelKills;
                LevelExitKills = 30;// Needed to leave the secrate level
                break;
        }
        // If exit kills don't reach then the door does not unlock
        if (SecretExit != null)
        {
            SecretExit.SetActive(false);
        }
        if (LevelExit != null)
        {
            LevelExit.SetActive(false);
        }

        if (ZombieKillText != null)
        {
            ZombieKillText.text = "Kills: 0/" + LevelExitKills;
        }
    }

    //Cloud Code//
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
    // Add kills to the kill count score
    public void AddKill()
    {
        ZombiesKilled++;

        if (ZombieKillText != null)
        {
            ZombieKillText.text = "Kills: " + ZombiesKilled + "/" + LevelExitKills;
        }        
        string scene = SceneManager.GetActiveScene().name;
        // To help save zombie kill count
        switch (scene)
        {
            case "Level 1":
                GameData.Level1Kills++;
                break;
            case "Level 2":
                GameData.Level2Kills++;
                break;
            case "Level 3":
                GameData.Level3Kills++;
                break;
            case "Secret Level":
                GameData.SecretLevelKills++;
                break;
        }

        // Unlock Secret Exit//
        if (!secretExitUnlocked && ZombiesKilled >= SecretExitKills)
        {
            secretExitUnlocked = true;
            Debug.Log("Secret Exit Unlocked!");
            if (SecretExit != null)
            {
                SecretExit.SetActive(true);
            }
        }
        // Unlock Normal Exit
        if (!LevelExitUnlocked && ZombiesKilled >= LevelExitKills)
        {
            LevelExitUnlocked = true;
            Debug.Log("Level Exit Unlocked!");
            if (LevelExit != null)
            {
                LevelExit.SetActive(true);
            }
        }
    }
    private void Game_Over()
    {
        IsDead = true;
        SceneManager.LoadScene("Game Over");
    }
    public void Update()
    {
        if (!Times_Up)
        {
            Gaming_Time -= Time.deltaTime;
            if (Gaming_Time <= 0 )
            {
                Gaming_Time = 0;
                Times_Up = true;
                Game_Over();
            }
        }
        if (Timer != null)
        {
            Timer.text = "Time: " + Mathf.CeilToInt(Gaming_Time);
        }
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
            gun.rotation = Quaternion.Euler(0, 0, 0);
            currentDirection = ShootDirection.Left;
            gun.localPosition = 
                new Vector3(
                -Mathf.Abs(gunStartposition.x), 
                gunStartposition.y, 
                gunStartposition.z);
        }
        // Move right and flip sprites connected to the player
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            moveX += 1f;
            SR.flipX = false;
            GunSR.flipX = false;
            gun.rotation = Quaternion.Euler(0, 0, 0);
            currentDirection = ShootDirection.Right;
            gun.localPosition =
                new Vector3(
                Mathf.Abs(gunStartposition.x), 
                gunStartposition.y, 
                gunStartposition.z);
        }
        // flips gun to go down
        if (Keyboard.current.downArrowKey.isPressed)
        {
            GunSR.flipX = false;
            gun.rotation = Quaternion.Euler(0, 0, -90);
            
            gun.localPosition =
                new Vector3(
                0f, 
                -Mathf.Abs(gunStartposition.y),
                gunStartposition.z);
            currentDirection = ShootDirection.Down;
        }
        // flips gun to go up
        if (Keyboard.current.upArrowKey.isPressed)
        {
            GunSR.flipX = false;
            gun.rotation = Quaternion.Euler(0, 0, 90);
            
            gun.localPosition =
                new Vector3(
                0f, 
                Mathf.Abs(gunStartposition.y),
                gunStartposition.z);
            currentDirection = ShootDirection.Up;
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
            GunTimer -= Time.deltaTime;
            Shoot();
            GunTimer = 3;
            if (AudioSource != null && Gunshot != null)
            {
                AudioSource.PlayOneShot(Gunshot);
            }
        }
        SR.transform.localScale = Vector3.Lerp(SR.transform.localScale, normalScale,10f * Time.deltaTime);
    }
    // Gun visibility here
    private void GunVisibility()
    {
        if (GunSR == null)
            return;
        if (GunTimer > 0f)
        {
            GunTimer -= Time.deltaTime;
            //Fades in
            GunColor.a = Mathf.Lerp(GunColor.a, 1f, GunFadingSpeed * Time.deltaTime);
            GunSR.color = GunColor;
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
        Vector3 spawnPosition = MuzzleRight.position;
        Vector2 velocity = Vector2.right * bulletSpeed;

        float bulletRotation = 0f;
        // Shows which way the muzzle, gunsnoke, gun and bullet directions depending on arrow keys and player direction
        switch (currentDirection)
        {
            case ShootDirection.Left:
                spawnPosition = MuzzleLeft.position;
                bulletRotation = 0f;
                velocity = Vector2.left * bulletSpeed;
                break;

            case ShootDirection.Right:
                spawnPosition = MuzzleRight.position;
                bulletRotation = 180f;
                velocity = Vector2.right * bulletSpeed;
                break;

            case ShootDirection.Down:
                bulletRotation = -90f;
                spawnPosition = MuzzleDown.position;
                velocity = Vector2.down * bulletSpeed;
                break;

            case ShootDirection.Up:
                bulletRotation = 90f;
                spawnPosition = MuzzleUp.position;
                velocity = Vector2.up * bulletSpeed;
                break;
        }
        BulletDust.transform.position = spawnPosition;
        BulletDust.Play();
        //Creates a new bullet after the first one//
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.Euler(0,0, bulletRotation));
        // Velocity Section
        if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
        {
            //Horizontal Velocity/Speed //
            bulletRb.linearVelocity = velocity;// 0f Means no vertical movement//
            //Add vertical movement later//
        }
        // flips the bullet sprite dependeing on player x axis
        if (bullet.TryGetComponent<Bullet>(out var bulletScript))
        {
            bulletScript.damage = damage;
        }
    }

}