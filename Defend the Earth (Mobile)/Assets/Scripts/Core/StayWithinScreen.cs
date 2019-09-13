using UnityEngine;

public class StayWithinScreen : MonoBehaviour
{
    void Update()
    {
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        float width = GetComponent<Collider>().bounds.extents.x;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenBounds.x * -1 + width, screenBounds.x - width), transform.position.y, 0);
    }
}