using UnityEngine;
using UnityEngine.SceneManagement;

public class Secret_Level : MonoBehaviour
{
    public static Secret_Level Instance;

    [Header("Kill Goal")]
    public int requiredKills = 30;

    private int zombieKills = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (SceneManager.GetActiveScene().name == "Secret Level")
            return;

        Playerscript player = other.GetComponent<Playerscript>();

        if (player.ZombiesKilled < player.SecretExitKillsCount)
        {
            Debug.Log("Secret Exit Locked!");
            return;
        }

        SceneManager.LoadScene("Secret Level");
    }
    public void ZombieKilled()
    {
        zombieKills++;

        Debug.Log($"Zombies Killed: {zombieKills}/{requiredKills}");

        if (zombieKills >= requiredKills)
        {
            Debug.Log("Exit Unlocked!");
        }
    }

}