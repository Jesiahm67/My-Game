using System.Collections;
using UnityEngine;

public class BossPortalSpawner : MonoBehaviour
{
    public GameObject GiantZombiePrefab;
    public SpriteRenderer BossSpawnPoint;
    public float spawnTime = 5f;
    public int maxGiantZombies = 5;
    public int GiantZombiePerPortal = 5;
    public float ActivationDistance = 150f;
    private float timer;
    private Transform Player;
    private int GiantZombieSpawned;
    private bool PortalClosed;
    private void Start()
    {
        timer = spawnTime;
        if (BossSpawnPoint != null)
        {
            Color color = BossSpawnPoint.color;
            color.a = 1f;
            BossSpawnPoint.color = color;
        }
        GameObject PlayerObject = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObject != null)
        {
            Player = PlayerObject.transform;
        }
    }
    private void Update()
    {
        if (PortalClosed)
            return;
        if (Player == null)
            return;
        float distance = Vector2.Distance(Player.position,transform.position);
        if (distance > ActivationDistance)
            return;
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnBoss();
            timer = spawnTime;
        }
    }
    private void SpawnBoss()
    {

        BossPortalSpawner[] giantZombies = FindObjectsByType<BossPortalSpawner>(FindObjectsSortMode.None);

        if (giantZombies.Length >= maxGiantZombies)
            return;
            
        Instantiate(
            GiantZombiePrefab,
            transform.position,
            Quaternion.identity);
        GiantZombieSpawned++;

        Debug.Log("Boss Zombie Spawned");
        if (GiantZombieSpawned >= GiantZombiePerPortal)
        {
            ClosePortal();
        }
    }
    private void ClosePortal()
    {
        PortalClosed = true;
        if (BossSpawnPoint != null)
        {
            StartCoroutine(FadePortal(BossSpawnPoint));
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
            color.a = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
            sprite.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position,ActivationDistance);
    }
}