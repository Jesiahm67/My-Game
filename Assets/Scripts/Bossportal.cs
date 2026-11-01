using System.Collections;
using UnityEngine;

public class BossPortalSpawner : MonoBehaviour
{
    public GameObject GiantZombiePrefab;

    public SpriteRenderer BossSpawnPoint;

    public float ActivationDistance = 60f;

    private Transform Player;

    private bool GiantZombieSpawned;
    private bool PortalClosed;

    private void Start()
    {
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
        if (GiantZombieSpawned)
            return;
        if (Player == null)
            return;
        float distance = Vector2.Distance(Player.position,transform.position);
        if (distance <= ActivationDistance)
        {
            SpawnBoss();
        }
    }
    private void SpawnBoss()
    {
        Instantiate(
            GiantZombiePrefab,
            transform.position,
            Quaternion.identity);
        GiantZombieSpawned = true;
        Debug.Log("Boss Zombie Spawned");
        ClosePortal();
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
            color.a = Mathf.Lerp(startColor.a,0f,elapsed / duration);
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