using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject GiantZombiePrefab;
    public Transform[] spawnPoints;

    //Time it takes for zombies to spawn from portals
    public float spawnTime = 1.5f;

    public int maxZombies = 10;

    // Number of zombies a portal can spawn before disappearing
    public int zombiesPerPortal = 10;

    public Transform barrier;
    // Distance when the portals chooses to spawn zombies
    public float activationDistance = 160f;

    private float timer;
    private Transform player;
    private bool giantZombieSpawned = false;

    private int[] portalSpawnCount;
    private bool[] portalClosed;

    void Start()
    {
        timer = spawnTime;

        portalSpawnCount = new int[spawnPoints.Length];
        portalClosed = new bool[spawnPoints.Length];

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        // Follow the player WITHOUT changing size
        if (player != null && barrier != null)
        {
            barrier.position = player.position;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnZombie();
            timer = spawnTime;
        }
    }

    void SpawnZombie()
    {
        if (player == null)
            return;

        Zombie[] zombies =
            FindObjectsByType<Zombie>(FindObjectsSortMode.None);

        if (zombies.Length >= maxZombies)
            return;

        Transform selectedSpawnPoint = null;
        int portalIndex = -1;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (portalClosed[i])
                continue;

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

        // Last portal = Giant Zombie only once
        if (portalIndex == spawnPoints.Length - 1)
        {
            if (!giantZombieSpawned)
            {
                Instantiate(
                    GiantZombiePrefab,
                    spawnPos,
                    Quaternion.identity);

                giantZombieSpawned = true;

                Debug.Log(
                    "Giant Zombie spawned at " +
                    selectedSpawnPoint.name);

                ClosePortal(portalIndex);
            }

            return;
        }

        // Normal zombie spawn
        Instantiate(
            zombiePrefab,
            spawnPos,
            Quaternion.identity);

        portalSpawnCount[portalIndex]++;

        Debug.Log(
            selectedSpawnPoint.name +
            " Spawn Count: " +
            portalSpawnCount[portalIndex]);

        // Fade portal after enough zombies spawned
        if (portalSpawnCount[portalIndex] >= zombiesPerPortal)
        {
            ClosePortal(portalIndex);
        }
    }

    void ClosePortal(int portalIndex)
    {
        if (portalClosed[portalIndex])
            return;

        portalClosed[portalIndex] = true;

        SpriteRenderer sprite =
            spawnPoints[portalIndex]
            .GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            StartCoroutine(FadePortal(sprite));
        }

        Debug.Log(
            spawnPoints[portalIndex].name +
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
}