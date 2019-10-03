using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private bool useInsideUnitSphere = false;
    [SerializeField] private float tumble = 0;
    [SerializeField] private Vector3 rotation = Vector3.zero;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (useInsideUnitSphere) rb.angularVelocity = Random.insideUnitSphere * tumble;
    }

    void Update()
    {
        if (!useInsideUnitSphere)
        {
            transform.Rotate(rotation * Time.deltaTime);
            rb.isKinematic = true;
        } else
        {
            rb.isKinematic = false;
        }
        rb.velocity = Vector3.zero;
    }
}
