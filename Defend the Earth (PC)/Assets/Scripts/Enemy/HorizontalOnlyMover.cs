using UnityEngine;

public class HorizontalOnlyMover : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f;

    private bool left = true;

    void Update()
    {
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        float width = GetComponent<Collider>().bounds.extents.x;
        Vector3 movement;
        if (left)
        {
            movement = new Vector3(-1, 0, 0).normalized * speed * Time.deltaTime;
        } else
        {
            movement = new Vector3(1, 0, 0).normalized * speed * Time.deltaTime;
        }
        transform.position += movement;
        if (transform.position.x <= screenBounds.x * -1 + width)
        {
            left = false;
        } else if (transform.position.x >= screenBounds.x - width)
        {
            left = true;
        }
    }
}