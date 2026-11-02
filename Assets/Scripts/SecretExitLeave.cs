using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretExitReturn : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Return to Level 1");

        SceneManager.LoadScene("Level 1");
    }
}