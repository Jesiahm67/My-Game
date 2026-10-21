using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teleportscript : MonoBehaviour

{
    // Make sure you check to match player location min and max on script from the gameobject you are using//
    public SpriteRenderer SR;

    public Vector3 minlocalation = new Vector3(-5, -5, -1);
    public Vector3 maxlocalation = new Vector3(5, 5, 1);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            TeleportRandomly();

        }
    }

    void TeleportRandomly()
    {

        float randomX = Random.Range(minlocalation.x, maxlocalation.x);
        float randomY = Random.Range(minlocalation.y, maxlocalation.y);
        float randomZ = Random.Range(minlocalation.z, maxlocalation.z);

        Vector3 randomPosition = new Vector3(randomX, randomY,randomZ);
        transform.position = randomPosition;
    }
}
