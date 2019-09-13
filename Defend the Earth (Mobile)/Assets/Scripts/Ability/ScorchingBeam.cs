using UnityEngine;

public class ScorchingBeam : MonoBehaviour
{
    [SerializeField] private long damage = 2;
    [SerializeField] private float timeBetweenHits = 0.02f;
    [SerializeField] private Transform hitGlow = null;
    public Transform origin;

    private LineRenderer line;
    private new BoxCollider collider;
    private float hitTime = 0;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        collider = GetComponent<BoxCollider>();
        if (PlayerPrefs.GetInt("Difficulty") <= 1) //Easy
        {
            damage = (long)(damage * 0.5);
        } else if (PlayerPrefs.GetInt("Difficulty") == 3)
        {
            timeBetweenHits *= 0.75f;
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            damage *= 2;
            timeBetweenHits *= 0.5f;
        }
    }

    void Update()
    {
        Transform start;
        if (origin)
        {
            start = origin;
        } else
        {
            start = transform;
        }
        Ray beamRay = new Ray(start.position, -transform.up);
        line.SetPosition(0, start.position);
        if (Physics.Raycast(beamRay, out RaycastHit beamHit, 50))
        {
            line.SetPosition(1, beamHit.point);
            if (hitGlow)
            {
                if (hitGlow.GetComponent<Light>()) hitGlow.GetComponent<Light>().enabled = true;
                hitGlow.gameObject.SetActive(true);
            }
        } else
        {
            line.SetPosition(1, beamRay.origin + beamRay.direction * 15);
            if (hitGlow)
            {
                if (hitGlow.GetComponent<Light>()) hitGlow.GetComponent<Light>().enabled = false;
                hitGlow.gameObject.SetActive(false);
            }
        }
        collider.size = new Vector3(line.endWidth, Vector3.Distance(line.GetPosition(0), line.GetPosition(1)));
        collider.transform.position = (line.GetPosition(0) + line.GetPosition(1)) / 2;
        float angle = Mathf.Atan2((line.GetPosition(1).z - line.GetPosition(0).z), (line.GetPosition(1).x - line.GetPosition(0).x));
        angle *= Mathf.Rad2Deg;
        angle *= -1;
        collider.transform.Rotate(0, angle, 0);
        if (hitGlow)
        {
            hitGlow.position = line.GetPosition(1);
            hitGlow.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (hitTime > 0) hitTime -= Time.deltaTime;
    }

    void OnTriggerStay(Collider other)
    {
        if (hitTime <= 0 && other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController)
            {
                playerController.takeDamage(damage);
                hitTime = timeBetweenHits;
            }
        }
    }
}
