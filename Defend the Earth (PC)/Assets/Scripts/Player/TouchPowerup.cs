using UnityEngine;

public class TouchPowerup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GameController.instance.gameOver && !GameController.instance.won)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController)
            {
                if (CompareTag("SmallRepair"))
                {
                    playerController.health += 15;
                } else if (CompareTag("LargeRepair"))
                {
                    playerController.health += 25;
                } else
                {
                    Debug.LogError("Powerup tag " + tag + " is invalid.");
                }
                Destroy(gameObject);
            } else
            {
                Debug.LogError("Could not find PlayerController!");
            }
        }
    }
}
