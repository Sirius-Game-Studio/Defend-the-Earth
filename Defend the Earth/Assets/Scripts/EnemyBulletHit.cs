using UnityEngine;

public class EnemyBulletHit : MonoBehaviour
{
    [Tooltip("Amount of damage dealt to players.")] public long damage = 3;

    void Update()
    {
        if (damage < 1) damage = 1; //Checks if damage is below 1
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController)
            {
                playerController.takeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}