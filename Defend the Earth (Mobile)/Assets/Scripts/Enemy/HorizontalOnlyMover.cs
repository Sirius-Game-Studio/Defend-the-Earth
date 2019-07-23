using UnityEngine;

public class HorizontalOnlyMover : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f;

    private bool direction = false;

    void Update()
    {
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        float width = GetComponent<Collider>().bounds.extents.x;
        Vector3 movement;
        if (!direction) //Left
        {
            movement = new Vector3(-1, 0, 0).normalized * speed * Time.deltaTime;
        } else //Right
        {
            movement = new Vector3(1, 0, 0).normalized * speed * Time.deltaTime;
        }
        transform.position += movement;
        if (transform.position.x <= screenBounds.x * -1 + width)
        {
            direction = true;
        } else if (transform.position.x >= screenBounds.x - width)
        {
            direction = false;
        }
    }
}
