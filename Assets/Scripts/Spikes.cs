using UnityEngine;

public class spikes : MonoBehaviour
{
    public int spikedamage = 100000000;

    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Playerscript>(out var player))
        {
            player.Damage(spikedamage);

        }
    }
}
