using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject GiantZombiePrefab;
    public Transform[] spawnPoints;

    //Time it takes for zombies to spawn from portals
    public float spawnTime = .5f;

    public int maxZombies = 10;

    // Number of zombies a portal can spawn before disappearing
    public int zombiesPerPortal = 10;

    // Distance when the portals chooses to spawn zombies
    public float activationDistance = 35f;

    private float timer;
    private Transform player;
    private int[] portalZombieSpawnCount;
    private bool[] portalClosed;// Sees if portal is closed
    private bool giantZombieSpawned = false;
    private bool playerEnteredSpawner = false;// Determines if the player is inside the spawnarea

    void Start()
    {
        timer = spawnTime;

        portalZombieSpawnCount = new int[spawnPoints.Length];
        portalClosed = new bool[spawnPoints.Length];
        //Uses player tag to find the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        //Stops spawning if the player is not in the spawnradius
        if (!playerEnteredSpawner)
            return;

        timer -= Time.deltaTime;
        // Invisible timer to tell when a zombie can come out of a portal
        if (timer <= 0f)
        {
            SpawnZombie();
            // Helps reset timer
            timer = spawnTime;
        }
        if (!giantZombieSpawned)
        {
            GiantZombieSpawned();
            giantZombieSpawned = true;

        }
    }
    void GiantZombieSpawned()
    {
        Debug.Log("Spawning Giant ZOmbie");
    }
    void SpawnZombie()
    {
        // Cannot spawn if player is not in game or in radius
        if (player == null)
            return;
        //Finds zombies in level/scene
        Zombie[] zombies = FindObjectsByType<Zombie>(FindObjectsSortMode.None);
        
        // If maximum point is reached the portal fades away
        if (zombies.Length >= maxZombies)
            return;

        Transform selectedSpawnPoint = null;
        int portalIndex = -1;
        // Searches through all portals even boss portal
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // Skip closed portals
            if (portalClosed[i])
                continue;
            // Demonstrate distance between player and portal
            float distance = Vector2.Distance(
                player.position,
                spawnPoints[i].position);

            if (distance <= activationDistance)
            {
                selectedSpawnPoint = spawnPoints[i];
                portalIndex = i;
                break;
            }
        }

        if (selectedSpawnPoint == null)
            return;

        Vector3 spawnPos = selectedSpawnPoint.position;
        spawnPos.z = 0f;
        //Boss portal code
        if (selectedSpawnPoint.CompareTag("Boss Portal"))
        {
            //Spawns boss zombie
            Instantiate(
                GiantZombiePrefab,
                spawnPos,
                Quaternion.identity);

            Debug.Log("Boss Zombie Spawned");

            return;
        }
        // Normal portal code
        if (selectedSpawnPoint.CompareTag("Zombie Portal"))
        {
            //Spawns normal zombie
            Instantiate(
                zombiePrefab,
                spawnPos,
                Quaternion.identity);
            // spawns zombie out of portal
            portalZombieSpawnCount[portalIndex]++;

            Debug.Log("Normal Zombie Spawned");
            // Checks to see if the portal can be closed
            if (portalZombieSpawnCount[portalIndex] >= zombiesPerPortal)
            {
                ClosePortal(portalIndex);
            }
        }


        // Fade portal after enough zombies spawned
        if (portalZombieSpawnCount[portalIndex] >= zombiesPerPortal)
        {
            ClosePortal(portalIndex);
        }
    }

    void ClosePortal(int portal_Index)
    {
        if (portalClosed[portal_Index])
            return;

        portalClosed[portal_Index] = true;

        SpriteRenderer sprite =
            spawnPoints[portal_Index]
            .GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            StartCoroutine(FadePortal(sprite));
        }

        Debug.Log(
            spawnPoints[portal_Index].name +
            " closed.");
    }

    IEnumerator FadePortal(SpriteRenderer sprite)
    {
        float duration = 2f;
        float elapsed = 0f;

        Color startColor = sprite.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Color color = startColor;
            color.a = Mathf.Lerp(
                startColor.a,
                0f,
                elapsed / duration);

            sprite.color = color;

            yield return null;
        }

        sprite.gameObject.SetActive(false);
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerEnteredSpawner = true;

            Debug.Log("Entered spawn radius");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerEnteredSpawner = false;

            Debug.Log("Left spawn radius");
        }

    }
}