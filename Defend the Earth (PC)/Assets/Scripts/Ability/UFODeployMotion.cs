using UnityEngine;

public class UFODeployMotion : MonoBehaviour
{
    public float y = 2;

    private EnemyGun enemyGun;
    private HorizontalOnlyMover mover;
    private Vector3 finalPosition;

    void Start()
    {
        enemyGun = GetComponent<EnemyGun>();
        mover = GetComponent<HorizontalOnlyMover>();
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
            enemyGun.enabled = false;
            mover.enabled = false;
        } else
        {
            enemyGun.enabled = true;
            mover.enabled = true;
            enabled = false;
        }
    }
}