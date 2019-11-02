using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private GameObject sound = null;

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
                    sound = null;
                    Debug.LogError("Powerup tag " + tag + " is invalid.");
                }
                if (sound) Instantiate(sound, transform.position, transform.rotation);
                Destroy(gameObject);
            } else
            {
                Debug.LogError("Could not find PlayerController!");
            }
        }
    }
}
