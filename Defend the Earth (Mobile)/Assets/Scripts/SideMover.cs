using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMover : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f;

    private Vector3 screenBounds = Vector3.zero;
    private float width = 0;
    private bool direction = false; //false is left, true is right

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        width = GetComponent<Collider>().bounds.extents.x;
    }

    void Update()
    {
        Vector3 movement;
        if (!direction)
        {
            movement = new Vector3(-1, 0, 0).normalized * speed * Time.deltaTime;
        } else
        {
            movement = new Vector3(1, 0, 0).normalized * speed * Time.deltaTime;
        }
        transform.position += movement;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenBounds.x * -1 + width, screenBounds.x - width), transform.position.y, 0);
        if (transform.position.x <= screenBounds.x * -1 + width)
        {
            direction = true;
        } else if (transform.position.x >= screenBounds.x - width)
        {
            direction = false;
        }
    }
}
