using UnityEngine;
using UnityEngine.InputSystem;

public class Hexagon : MonoBehaviour
{
    public float speed = 5f; // Units per second
    public int Health = 5;

    public Transform Moveit;

    public Vector3 Dhir = new Vector3(5, 1, 2);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.rightArrowKey.isPressed)
        {

            Moveit.position += new Vector3(0.1f, 0, 0);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {

            Moveit.position -= new Vector3(0.1f, 0, 0);
        }
        if (Keyboard.current.upArrowKey.isPressed)
        {

            Moveit.position += new Vector3(0, 0.1f, 0);
        }
        if (Keyboard.current.downArrowKey.isPressed)
        {

            Moveit.position -= new Vector3(0, 0.1f, 0);
        }
        transform.position += speed * Time.deltaTime * transform.right;
        Vector3 pos = transform.position;
        pos.x = 8f;
        transform.position = pos;

    }
}
