using System.Collections;
using UnityEngine;

public class BossPortalSpawner : MonoBehaviour
{
    public GameObject giantZombiePrefab;

    public SpriteRenderer BossSpawnPoint;

    public float activationDistance = 60f;

    private Transform player;

    private bool giantZombieSpawned;
    private bool portalClosed;

    private void Start()
    {
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
        if (giantZombieSpawned)
            return;
        if (player == null)
            return;
        float distance = Vector2.Distance(player.position,transform.position);
        if (distance <= activationDistance)
        {
            SpawnBoss();
        }
    }
    private void SpawnBoss()
    {
        Instantiate(
            giantZombiePrefab,
            transform.position,
            Quaternion.identity);
        giantZombieSpawned = true;
        Debug.Log("Boss Zombie Spawned");
        ClosePortal();
    }

    private void ClosePortal()
    {
        portalClosed = true;

        if (BossSpawnPoint != null)
        {
            StartCoroutine(
                FadePortal(BossSpawnPoint));
        }
    }

    private IEnumerator FadePortal(
        SpriteRenderer sprite)
    {
        float duration = 2f;
        float elapsed = 0f;
        Color startColor = sprite.color;
        while (elapsed < duration)
        {   elapsed += Time.deltaTime;
            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a,0f,elapsed / duration);
            sprite.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position,activationDistance);
    }
}