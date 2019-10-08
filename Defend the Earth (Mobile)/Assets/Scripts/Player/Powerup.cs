using UnityEngine;

public class Powerup : MonoBehaviour
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
                    playerController.repair(playerController.smallRepairHeal);
                } else if (CompareTag("LargeRepair"))
                {
                    playerController.repair(playerController.largeRepairHeal);
                } else if (CompareTag("Supercharge"))
                {
                    playerController.supercharge();
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
