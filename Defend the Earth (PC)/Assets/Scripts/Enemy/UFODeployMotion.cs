using UnityEngine;

public class UFODeployMotion : MonoBehaviour
{
    public float y = 2.5f;

    private Vector3 finalPosition;
    //private float time = 0;

    void Start()
    {
        if (y < 0)
        {
            y = -y;
        } else if (y == 0)
        {
            y = 2.5f;
        }
        finalPosition = transform.position - new Vector3(0, y, 0);
    }

    void Update()
    {
        if (transform.position.y > finalPosition.y)
        {
            transform.position -= new Vector3(0, 1, 0) * 1.5f * Time.deltaTime;
            GetComponent<EnemyGun>().enabled = false;
            GetComponent<SideMover>().enabled = false;
        } else
        {
            GetComponent<EnemyGun>().enabled = true;
            GetComponent<SideMover>().enabled = true;
            enabled = false;
            /*
            if (time < 2)
            {
                transform.Rotate(new Vector3(-45, 0, 0) * Time.deltaTime);
                time += Time.deltaTime;
            } else
            {
                transform.rotation = Quaternion.Euler(-90, transform.rotation.y, transform.rotation.z);
                GetComponent<EnemyGun>().enabled = true;
                GetComponent<SideMover>().enabled = true;
                enabled = false;
            }
            */
        }
    }
}