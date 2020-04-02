using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDeathEffect : MonoBehaviour
{
    [Tooltip("This all-purpose script uses versatile variables to achieve effects.\n\nCurrently implemented behaviors (format is : 'name (value1, value2)') :\n - explode (damage, range): Damages all enemies within range by damage. Damage decays such that enemies at the center take 100% damage and enemies on the edge will take near 0%.")]

    public string effect;
    public float value1, value2;
    public GameObject spawnedOnDeath, deathParticle;

    public void Run()
    {
        if (spawnedOnDeath != null)
        {
            GameObject explosionInstance = Instantiate(spawnedOnDeath, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 5);
        }
        if (deathParticle != null)
        {
            GameObject explosionInstance = Instantiate(spawnedOnDeath, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 2);
        }
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, value2);
        foreach (Collider col in collidersInRange)
        {
            if (col && col.tag == "Enemy" && (col.gameObject.GetComponent<EnemyAttributes>().GetIsDead() == false))
            {
                col.gameObject.GetComponent<EnemyAttributes>().DealDamage(value1 * ((value2- (col.gameObject.transform.position - transform.position).magnitude) / value2));
            }
        }
    }
}
