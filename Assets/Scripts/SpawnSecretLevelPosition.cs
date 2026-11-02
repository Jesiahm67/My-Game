using UnityEngine;
public class SpawnSecretLevelPosition : MonoBehaviour
{


    public Transform SecretReturnSpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!GameData.ReturningFromSecretLevel)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = SecretReturnSpawn.position;
        }
        GameData.ReturningFromSecretLevel = false;
    }
}
