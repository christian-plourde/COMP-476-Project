using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BastardSword : MonoBehaviour
{
    [Range(0, 100)]
    public int criticalChance;
    public int baseDMG=4;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            // roll critical
            int dmg = baseDMG;
            int r = Random.Range(0, 100);
            if (r < criticalChance)
            {
                dmg += baseDMG * criticalChance;
            }

            other.GetComponent<EnemyAttributes>().DealDamage(dmg);
        }
    }
}
