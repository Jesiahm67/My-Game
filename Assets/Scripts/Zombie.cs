using UnityEngine;

public class Zombie : MonoBehaviour
{
    public Transform target;

    public int damage = 5;
    public float followSpeed = 20f;
    public float Health = 10f;

    public float jumpForce = 17.5f;

    private Rigidbody2D rig;
    private bool isGrounded;

    private float facingDirection = 1f;
    private float jumpCooldown = 0.5f;
    private float nextJumpTime;

    private void OnEnable()
    {
        Playerscript.Death += Stopfollowing;
    }

    private void OnDisable()
    {
        Playerscript.Death -= Stopfollowing;
    }

    private void Stopfollowing()
    {
        target = null;

        if (rig != null)
        {
            rig.linearVelocity = Vector2.zero;
        }
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (target == null)
            return;

        float distanceX = target.position.x - transform.position.x;

        // Follow player
        rig.linearVelocity = new Vector2(
            facingDirection * followSpeed,
            rig.linearVelocity.y
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.TryGetComponent<Playerscript>(out var player))
        {
            player.Damage(damage);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}