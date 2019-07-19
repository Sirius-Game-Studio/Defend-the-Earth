using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (CompareTag("Boundary"))
        {
            if (other.CompareTag("Enemy") && !GameController.instance.gameOver && !GameController.instance.won) ++GameController.instance.aliensReached;
            if (!other.CompareTag("Player")) Destroy(other.gameObject);
        }
    }
}