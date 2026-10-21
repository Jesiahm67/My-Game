using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform Player;
    public Vector3 offset = new(0, 0, -5f);
    public float steadyspeed = 0.125f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Player = player.transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Player == null)
            return;

        Vector3 targetPosition = Player.position + offset;
        Vector3 steadypositon = Vector3.Lerp(transform.position, targetPosition, steadyspeed);
        transform.position = steadypositon;


    }
}
