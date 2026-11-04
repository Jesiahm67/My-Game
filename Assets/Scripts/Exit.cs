using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        Playerscript player = other.GetComponent<Playerscript>();

        if (player.ZombiesKilled < player.SecretExitKillsCount)
        {
            Debug.Log("Secret Exit Locked! Need " + (player.SecretExitKillsCount - player.ZombiesKilled) + " more zombie Kills");
            return;
        }
        Debug.Log("Entering Secret Level");
        SceneManager.LoadScene("Secret Level");


        int CurrentScene = SceneManager.GetActiveScene().buildIndex;
            int nextScene = CurrentScene + 1;

            if (nextScene < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                SceneManager.LoadScene("Victory");
            }
        
    }
}
