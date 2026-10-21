using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;

    public float spawnTime = 1.5f;
    public int maxZombies = 10;

    // Distance at which spawning becomes active
    public float activationDistance = 25f;

    private float timer;
    private Transform player;

    void Start()
    {
        timer = spawnTime;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnZombie();
            timer = spawnTime;
        }
    }

    void SpawnZombie()
    {
        Zombie[] zombies =
            FindObjectsByType<Zombie>(FindObjectsSortMode.None);

        if (zombies.Length >= maxZombies)
            return;

        // Find all spawn points close enough to the player
        Transform selectedSpawnPoint = null;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            float distance =
                Vector2.Distance(
                    player.position,
                    spawnPoints[i].position);

            if (distance <= activationDistance)
            {
                selectedSpawnPoint = spawnPoints[i];
                break;
            }
        }

        // No spawn point is close enough
        if (selectedSpawnPoint == null)
            return;

        Vector3 spawnPos = selectedSpawnPoint.position;
        spawnPos.z = 0f;

        Instantiate(
            zombiePrefab,
            spawnPos,
            Quaternion.identity);

        Debug.Log(
            "Zombie spawned at " +
            selectedSpawnPoint.name);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null)
            return;

        Gizmos.color = Color.red;

        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(
                    point.position,
                    activationDistance);
            }
        }
    }
}