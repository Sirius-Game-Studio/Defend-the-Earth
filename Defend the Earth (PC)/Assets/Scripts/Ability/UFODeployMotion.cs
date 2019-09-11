using UnityEngine;

public class UFODeployMotion : MonoBehaviour
{
    public float y = 2;

    private Vector3 finalPosition;

    void Start()
    {
        if (y < 0)
        {
            y = -y;
        } else if (y == 0)
        {
            y = 2;
        }
        finalPosition = transform.position - new Vector3(0, y, 0);
    }

    void Update()
    {
        if (transform.position.y > finalPosition.y)
        {
            transform.position -= new Vector3(0, 1, 0) * 1.5f * Time.deltaTime;
            GetComponent<EnemyGun>().enabled = false;
            GetComponent<HorizontalOnlyMover>().enabled = false;
        } else
        {
            GetComponent<EnemyGun>().enabled = true;
            GetComponent<HorizontalOnlyMover>().enabled = true;
            enabled = false;
        }
    }
}