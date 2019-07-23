using UnityEngine;

public class Mover : MonoBehaviour
{
    [Tooltip("How fast this object moves.")] public float speed = 2.5f;
    [Tooltip("Where the object should move towards, if it's not using transform.forward.")] [SerializeField] private Vector3 movement = Vector3.down;
    [Tooltip("Should this object move, using transform.forward?")] [SerializeField] private bool useForwardMovement = true;

    void Update()
    {
        if (useForwardMovement) //Moves the object using transform.forward
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        } else //Moves the object using set movement values
        {
            transform.position += movement * speed * Time.deltaTime;
        }
    }
}