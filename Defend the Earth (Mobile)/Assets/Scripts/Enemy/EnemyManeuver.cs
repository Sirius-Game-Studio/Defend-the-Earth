using System.Collections;
using UnityEngine;

public class EnemyManeuver : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private Vector2 timeTillManeuver = new Vector2(4.5f, 6);
    [SerializeField] private Vector2 maneuverTime = new Vector2(0.5f, 1);

    private Mover mover;
    private float maneuver = 0;
    private bool left = true;

    void Start()
    {
        mover = GetComponent<Mover>();
        if (mover) speed = mover.speed;
        if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            maneuverTime *= 1.05f;
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            timeTillManeuver *= 0.85f;
            maneuverTime *= 1.1f;
        }
        StartCoroutine(doManeuvers());
    }

    void Update()
    {
        if (maneuver > 0)
        {
            Vector3 movement;
            if (left)
            {
                movement = Vector3.left;
            } else
            {
                movement = Vector3.right;
            }
            transform.position += movement * speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            maneuver -= Time.deltaTime;
        }
    }

    IEnumerator doManeuvers()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(timeTillManeuver.x, timeTillManeuver.y));
            float random = Random.value;
            if (random <= 0.5f)
            {
                left = true;
            } else
            {
                left = false;
            }
            maneuver = Random.Range(maneuverTime.x, maneuverTime.y);
        }
    }
}
