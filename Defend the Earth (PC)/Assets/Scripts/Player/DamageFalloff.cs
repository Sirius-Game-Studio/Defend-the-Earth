using UnityEngine;

public class DamageFalloff : MonoBehaviour
{
    [SerializeField] private long minimumDamage = 3;
    [SerializeField] private long damageDecrement = 1;
    [SerializeField] private float falloffTime = 0.8f;

    private BulletHit bulletHit;

    void Start()
    {
        bulletHit = GetComponent<BulletHit>();
        if (!bulletHit)
        {
            Debug.LogError("Could not find BulletHit in GameObject " + name + "!");
            return;
        }
        if (bulletHit) InvokeRepeating("dropDamage", falloffTime, falloffTime);
    }

    void Update()
    {
        if (bulletHit.damage < minimumDamage) damageDecrement = minimumDamage;
        if (minimumDamage < 0) minimumDamage = 0; //Checks if minimum damage is below 0
        if (damageDecrement < 1) damageDecrement = 1; //Checks if damage decrement is below 1
    }

    void dropDamage()
    {
        if (bulletHit.damage > minimumDamage)
        {
            bulletHit.damage -= damageDecrement;
            print(bulletHit.damage);
        } else if (bulletHit.damage < minimumDamage)
        {
            bulletHit.damage = damageDecrement;
            CancelInvoke("dropDamage");
        }
    }
}
