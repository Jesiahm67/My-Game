using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]

public class Playerscript : MonoBehaviour
{
    public static event Action Death;
    public AudioSource PlayerAudioSource;
    public TextMeshPro ZombieDeathText;

    [Header("Health")]
    public int Maximum_Health = 100;
    public int Health;
    public TextMeshPro HealthText;

    [Header("Time")]
    public float Gaming_Time = 300f;
    private bool Times_Up = false;
    public TextMeshPro Timer;

    [Header("Score")]
    public int ZombiesKilled = 0;

    [Header("Kill Goals")]
    public int SecretExitKillsCount = 25;
    public int LevelExitKillCount = 55;
    public GameObject SecretExit;
    public GameObject LevelExit;
    private bool issecretExitUnlocked;
    private bool isLevelExitUnlocked;


    public bool AreuDead { get; private set; }

    [Header("Components")]
    public SpriteRenderer SR;
    public SpriteRenderer GunSR;

    [Header("Movement")]
    public float MaxSpeed = 20f;
    public float NormalSpeed;
    public float SprintAccelerator = 2.5f;



    [Header("Jumping")]
    public float MaxJumpingForce = 50f;
    public float JumpingForce;
    public ParticleSystem JumpDust;
    public AudioClip JumpSound;

    private Vector3 normalScale;


    [Header("Cloud Bounce")]
    public float CloudFloatingflight; 
    public float CloudFloatingDuatin;
    private bool IsthePlayerFloating;
    private float floatime;

    [Header("Pistol")]
    public GameObject bulletPrefabHere;
    public ParticleSystem BulletDust;
    public AudioClip Gunshot;
    public Transform gun;
    public Transform MuzzleRight;
    public Transform MuzzleLeft;
    public Transform MuzzleDown;
    public Transform MuzzleUp;
    public float bulletSpeed = 200f;
    public float damage = 20f;
    private readonly float GunVisibilityTime = 2f;
    private readonly float GunFadingSpeed = 3f;
    private float GunTimer;
    private Color GunColor;
    private Vector3 gunStartposition;
    private enum ShootDirection { Left, Right, Up, Down }
    private ShootDirection currentDirection = ShootDirection.Right;

    private Rigidbody2D Rigidbody;

    private int Jumpsleft;
    private const int MaxJumpsleft = 2;

    public void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        Health = Maximum_Health;
        NormalSpeed = MaxSpeed;
        JumpingForce = MaxJumpingForce;
        normalScale = SR.transform.localScale;
        gunStartposition = gun.localPosition;

        Jumpsleft = MaxJumpsleft;

        AreuDead = false;

        if (SR == null)
        {
            SR = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        }
        if (GunSR != null)
        {
            GunColor = GunSR.color;
            GunColor.a = 0f;
            GunSR.color = GunColor;
        }
        if (PlayerAudioSource == null)
        {
            PlayerAudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        }

        UpdateHealth();

        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "Level 1":
                ZombiesKilled = GameData.Level1Kills;
                SecretExitKillsCount = 30;
                LevelExitKillCount = 55;
                break;
            case "Level 2":
                ZombiesKilled = GameData.Level2Kills;
                SecretExitKillsCount = 60;
                LevelExitKillCount = 65;
                break;
            case "Level 3":
                ZombiesKilled = GameData.Level3Kills;
                SecretExitKillsCount = 70;
                LevelExitKillCount = 75;
                break;
            case "Secret Level":
                ZombiesKilled = GameData.SecretLevelKills;
                LevelExitKillCount = 30;
                break;
        }
        if (SecretExit != null)
        {
            SecretExit.SetActive(false);
        }
        if (LevelExit != null)
        {
            LevelExit.SetActive(false);
        }

        if (ZombieDeathText != null)
        {
            ZombieDeathText.text = "Kills: 0/" + LevelExitKillCount;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Cloud"))
        {
            Debug.Log("Touched Cloud!");
            IsthePlayerFloating = true;
            floatime = CloudFloatingDuatin;

            Jumpsleft = MaxJumpsleft;
            return;
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                Jumpsleft = MaxJumpsleft;
                break;
            }
        }
    }

    public void Damage(int damage)
    {
        if (AreuDead )
            return;

        Health = Mathf.Max(0, Health - damage);

        UpdateHealth();

        Debug.Log("Health: " + Health);

        if (Health == 0)
        {
            AreuDead = true;

            if (SR != null)
            {
                SR.color = Color.red;
            }

            Death?.Invoke();

            Debug.Log("Player Died");
            SceneManager.LoadScene("Game Over");

        }
    }
    public void AddKill()
    {
        ZombiesKilled++;

        if (ZombieDeathText != null)
        {
            ZombieDeathText.text = "Kills: " + ZombiesKilled + "/" + LevelExitKillCount;
        }
        string scene = SceneManager.GetActiveScene().name;
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

        if (!issecretExitUnlocked && ZombiesKilled >= SecretExitKillsCount)
        {
            issecretExitUnlocked = true;
            Debug.Log("Secret Exit Unlocked!");
            if (SecretExit != null)
            {
                SecretExit.SetActive(true);
            }
        }
        if (!isLevelExitUnlocked && ZombiesKilled >= LevelExitKillCount)
        {
            isLevelExitUnlocked = true;
            Debug.Log("Level Exit Unlocked!");
            if (LevelExit != null)
            {
                LevelExit.SetActive(true);
            }
        }
    }
    private void Game_Over()
    {
        AreuDead = true;
        SceneManager.LoadScene("Game Over");
    }
    public void Update()
    {
        if (!Times_Up)
        {
            Gaming_Time -= Time.deltaTime;
            if (Gaming_Time <= 0)
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
        if (IsthePlayerFloating)
        {
            floatime -= Time.deltaTime;

            Rigidbody.linearVelocity = new Vector2(
                Rigidbody.linearVelocity.x,
                CloudFloatingflight);
            if (floatime <= 0f)
            {
                IsthePlayerFloating = false;
            }
        }
        GunVisibility();

        if (AreuDead)
            return;

        if (Keyboard.current == null)
            return;

        float moveX = 0f;

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
        float currentSpeed = NormalSpeed;
        if (Keyboard.current.shiftKey.isPressed)
        {
            currentSpeed *= SprintAccelerator;
        }
        Rigidbody.linearVelocity = new Vector2(
            moveX * currentSpeed,
            Rigidbody.linearVelocity.y
        );

        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            Jumpsleft > 0)
        {
            Rigidbody.linearVelocity = new Vector2(
                Rigidbody.linearVelocity.x,
                JumpingForce
            );
            SR.transform.localScale = new Vector3(normalScale.x * 0.95f, normalScale.y * 1.05f, normalScale.z);
            Jumpsleft--;

            if (JumpDust != null)
            {
                JumpDust.Play();
            }
            if (PlayerAudioSource != null && JumpSound != null)
            {
                PlayerAudioSource.PlayOneShot(JumpSound);
            }
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            GunTimer -= Time.deltaTime;
            Shoot();
            GunTimer = 3;
            if (PlayerAudioSource != null && Gunshot != null)
            {
                PlayerAudioSource.PlayOneShot(Gunshot);
            }
        }
        SR.transform.localScale = Vector3.Lerp(SR.transform.localScale, normalScale, 10f * Time.deltaTime);
    }
    private void GunVisibility()
    {
        if (GunSR == null)
            return;
        if (GunTimer > 0f)
        {
            GunTimer -= Time.deltaTime;
            GunColor.a = Mathf.Lerp(GunColor.a, 1f, GunFadingSpeed * Time.deltaTime);
            GunSR.color = GunColor;
        }
        else
        {
            GunColor.a = Mathf.Lerp(GunColor.a, 0f, GunFadingSpeed * Time.deltaTime);
            GunSR.color = GunColor;
        }
    }
    public void UpdateHealth()
    {
        if (HealthText != null)
        {
            HealthText.text = "Health: " + Health;
        }
    }
    private void Shoot()
    {
        GunTimer = GunVisibilityTime;
        GunColor.a = 1f;
        GunSR.color = GunColor;
        if (bulletPrefabHere == null || gun == null)
            return;
        Vector3 spawnPoint = MuzzleRight.position;
        Vector2 Speed = Vector2.right * bulletSpeed;
        float bulletRotatin = 0f;
        switch (currentDirection)
        {   case ShootDirection.Up:
                bulletRotatin = 90f;
                spawnPoint = MuzzleUp.position;
                Speed = Vector2.up *bulletSpeed;
                break;

            case ShootDirection.Down:
                bulletRotatin = -90f;
                spawnPoint = MuzzleDown.position;
                Speed = Vector2.down *bulletSpeed;
                break;

            case ShootDirection.Left:
                spawnPoint = MuzzleLeft.position;
                bulletRotatin = 0f;
                Speed = Vector2.left *bulletSpeed;
                break;

            case ShootDirection.Right:
                spawnPoint = MuzzleRight.position;
                bulletRotatin = 180f;
                Speed = Vector2.right *bulletSpeed;
                break;
        }
        BulletDust.transform.position=spawnPoint;
        BulletDust.Play();
        GameObject bullet = Instantiate(bulletPrefabHere, spawnPoint, Quaternion.Euler(0, 0, bulletRotatin));
        if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
        {
            bulletRb.linearVelocity = Speed;
        }
        if (bullet.TryGetComponent<Bullet>(out var bulletScript))
        {
            bulletScript.damage = damage;
        }
    }

}