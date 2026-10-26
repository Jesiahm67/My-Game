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
        // Ignore other bullets
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            return;
        }
        if(other.TryGetComponent<Zombie>(out var zombie))
        {
            zombie.Damage((int)damage);
            Destroy(gameObject);
        }
    }
}