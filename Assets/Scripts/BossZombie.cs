using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossZombie : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 35;
    public float attackCooldown = 1f;
    private float nextAttackTime;
    public int health = 100;
    public float moveSpeed = 7.5f;
    public float stopDistance = 1f;
    private Rigidbody2D rb;
    private Playerscript player;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<Playerscript>();
        }
    }

    private void FixedUpdate()
    {
        if (player == null || player.AreuDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance =
            player.transform.position.x - transform.position.x;

        if (Mathf.Abs(distance) > stopDistance)
        {
            float direction = Mathf.Sign(distance);

            rb.linearVelocity = new Vector2(
                direction * moveSpeed,
                rb.linearVelocity.y
            );
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }
        else
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y);
        }
    }

    public void Damage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (player == null)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                player.Damage(damage);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }
    private void Die()
    {
        if (player != null)
        {
            player.AddKill();
        }
        Destroy(gameObject);
    }
}