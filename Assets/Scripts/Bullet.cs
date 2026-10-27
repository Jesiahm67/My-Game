using System.IO.Compression;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;
    public float damage;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        // Ignore other bullets
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
            return;
        //Hits zombie
        if(other.TryGetComponent<Zombie>(out var zombie))
        {
            zombie.Damage((int)damage);
            Destroy(gameObject);
            return;
        }
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        if (other.CompareTag("Boss Portal"))
        {
            Destroy(gameObject);
        }
        if (other.CompareTag("Zombie Portal"))
        {
            Destroy(gameObject);
        }
        if (other.CompareTag("Secret Exit"))
        {
            Destroy(gameObject);
        }
        if (other.CompareTag("Finish"))
        {
            Destroy(gameObject);
        }
    }
}