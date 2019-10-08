using UnityEngine;

public class DamageFalloff : MonoBehaviour
{
    [SerializeField] private long minimumDamage = 3;
    [SerializeField] private long damageDecrement = 1;
    [SerializeField] private float falloffTime = 0.8f;

    private Bullet bulletHit;

    void Start()
    {
        bulletHit = GetComponent<Bullet>();
        InvokeRepeating("dropDamage", falloffTime, falloffTime);
    }

    void Update()
    {
        if (bulletHit.damage < minimumDamage) bulletHit.damage = minimumDamage; //Checks if damage is less than the minimum
        if (minimumDamage < 0) minimumDamage = 0; //Checks if minimum damage is less than 0
        if (damageDecrement < 1) damageDecrement = 1; //Checks if damage decrement is less than 1
    }

    void dropDamage()
    {
        if (bulletHit.damage != minimumDamage)
        {
            bulletHit.damage -= damageDecrement;
        } else
        {
            CancelInvoke("dropDamage");
        }
    }
}
