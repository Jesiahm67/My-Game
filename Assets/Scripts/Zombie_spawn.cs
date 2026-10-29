using System.Collections;
using UnityEngine;

public class ZombiePortalSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public SpriteRenderer ZombieSpawnPoint;

    public float spawnTime = .5f;
    public int maxZombies = 10;
    public int zombiesPerPortal = 10;
    public float activationDistance = 35f;

    private float timer;
    private Transform player;

    private int zombiesSpawned;
    private bool portalClosed;

    private void Start()
    {
        timer = spawnTime;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (portalClosed)
            return;

        if (player == null)
            return;

        float distance =
            Vector2.Distance(
            player.position,
            transform.position);

        if (distance > activationDistance)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
        SpawnZombie();
            timer = spawnTime;
        }
    }
    private void SpawnZombie()
    {
        Zombie[] zombies =
            FindObjectsByType<Zombie>(
                FindObjectsSortMode.None);

        if (zombies.Length >= maxZombies)
            return;

        Instantiate(
            zombiePrefab,
            transform.position,
            Quaternion.identity);

        zombiesSpawned++;

        Debug.Log("Normal Zombie Spawned");

        if (zombiesSpawned >= zombiesPerPortal)
        {
            ClosePortal();
        }
    }
    private void ClosePortal()
    {
        portalClosed = true;

        if (ZombieSpawnPoint != null)
        {
            StartCoroutine(
                FadePortal(ZombieSpawnPoint));
        }
    }
    private IEnumerator FadePortal(
        SpriteRenderer sprite)
    {
        float duration = 2f;
        float elapsed = 0f;
        Color startColor = sprite.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a,0f,elapsed / duration);
            sprite.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,activationDistance);
    }
}