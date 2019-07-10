using UnityEngine;

public class EnemyBulletHit : MonoBehaviour
{
    [Tooltip("Amount of damage dealt to players.")] public int damage = 1;
    [SerializeField] private GameObject explosion;

    private bool hit = false;

    void Update()
    {
        if (damage < 1) damage = 1; //Checks if damage is below 1
    }

    void OnTriggerStay(Collider other)
    {
        if (!hit && other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController)
            {
                playerController.takeDamage(damage);
                if (explosion) Instantiate(explosion, transform.position, transform.rotation);
                hit = true;
                Destroy(gameObject);
            }
        }
    }
}