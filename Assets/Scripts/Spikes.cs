using UnityEngine;

public class spikes : MonoBehaviour
{
    public int spikedamage = 100000000;

    public Transform target;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Playerscript>(out var player))
        {
            player.Damage(spikedamage);

        }
    }
}
