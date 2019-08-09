using UnityEngine;
using TMPro;

public class TouchPowerup : MonoBehaviour
{
    [SerializeField] private GameObject textPopup = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GameController.instance.gameOver && !GameController.instance.won)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController)
            {
                if (CompareTag("SmallRepair"))
                {
                    playerController.health += playerController.smallRepairHeal;
                    if (textPopup)
                    {
                        if (textPopup.GetComponent<TextMeshPro>())
                        {
                            GameObject popup = Instantiate(textPopup, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.Euler(0, 0, 0));
                            popup.GetComponent<TextMeshPro>().text = "+" + playerController.smallRepairHeal;
                            popup.GetComponent<TextMeshPro>().color = new Color32(0, 255, 0, 255);
                            popup.GetComponent<TextMeshPro>().outlineColor = new Color32(0, 127, 0, 255);
                        } else
                        {
                            Debug.LogError("TextPopup object does not have a TextMeshPro component!");
                        }
                    }
                } else if (CompareTag("LargeRepair"))
                {
                    playerController.health += playerController.largeRepairHeal;
                    if (textPopup)
                    {
                        if (textPopup.GetComponent<TextMeshPro>())
                        {
                            GameObject popup = Instantiate(textPopup, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.Euler(0, 0, 0));
                            popup.GetComponent<TextMeshPro>().text = "+" + playerController.largeRepairHeal;
                            popup.GetComponent<TextMeshPro>().color = new Color32(0, 255, 0, 255);
                            popup.GetComponent<TextMeshPro>().outlineColor = new Color32(0, 127, 0, 255);
                        } else
                        {
                            Debug.LogError("TextPopup object does not have a TextMeshPro component!");
                        }
                    }
                } else if (CompareTag("Supercharge"))
                {
                    playerController.supercharge();
                    if (textPopup)
                    {
                        if (textPopup.GetComponent<TextMeshPro>())
                        {
                            GameObject popup = Instantiate(textPopup, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.Euler(0, 0, 0));
                            popup.GetComponent<TextMeshPro>().text = "Supercharge!";
                            popup.GetComponent<TextMeshPro>().color = new Color32(0, 255, 255, 255);
                            popup.GetComponent<TextMeshPro>().outlineColor = new Color32(0, 127, 127, 255);
                        } else
                        {
                            Debug.LogError("TextPopup object does not have a TextMeshPro component!");
                        }
                    }
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
