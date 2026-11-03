using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Title Screen");
        }
    }
}
