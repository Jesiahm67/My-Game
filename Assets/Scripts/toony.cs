using UnityEngine;
using UnityEngine.InputSystem;

public class toony : MonoBehaviour


{
    public Transform Moveit;
    public float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Keyboard.current.rightArrowKey.isPressed)
        {

            Moveit.position += new Vector3(0.05f, 0, 0);
        }
        else if (Keyboard.current.leftArrowKey.isPressed)
        {
            Moveit.position -= new Vector3(0.05f, 0, 0);
        }
        if (Keyboard.current.upArrowKey.isPressed)
        {

            Moveit.position += new Vector3(0, 0.05f, 0);
        }
        else if (Keyboard.current.downArrowKey.isPressed)
        {

            Moveit.position -= new Vector3(0, 0.05f, 0);
        }
        if (transform.position.x > 37f)
        {
            Vector3 pos = transform.position;
            pos.x = -37f;
            transform.position = pos;
        }
        else if (transform.position.x < -37f)
        {
            Vector3 pos = transform.position;
            pos.x = 37f;
            transform.position = pos;

        }
        if (transform.position.y > 25f)
        {
            Vector3 pos = transform.position;
            pos.y = -25f;
            transform.position = pos;
        }
        else if (transform.position.y < -25f)
        {
            Vector3 pos = transform.position;
            pos.y = 25f;
            transform.position = pos;

        }
        else
        {
            transform.position += speed * Time.deltaTime * transform.right;
        }
    }
}
