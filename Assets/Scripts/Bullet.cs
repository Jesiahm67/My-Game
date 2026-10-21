using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore other bullets
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            return;
        }

        if (other.CompareTag("Zombie"))
        {
            Debug.Log("Zombie Killed");

            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}