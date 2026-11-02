using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretExitReturn : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameData.ReturningFromSecretLevel = true;

        SceneManager.LoadScene("Level 1");
    }
}