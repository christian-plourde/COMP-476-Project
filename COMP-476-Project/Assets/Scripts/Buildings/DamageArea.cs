using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public float damage, range, cooldown;

    private int scanTimer, cooldownTimer;

    public GameObject explosion;

    void Start()
    {
        // Convert from seconds to frames
        cooldown *= 60;
    }

    void Update()
    {
        scanTimer++;
        if (cooldownTimer < cooldown)
        {
            cooldownTimer++;
        }
        else
        {
            if (scanTimer >= 20)
            {
                scanTimer = 0;
                Pulse();
            }
        }       
    }

    void Pulse()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, range);
        foreach (Collider col in collidersInRange)
        {
            if (col && validTarget(col.gameObject) == true)
            {
                if (explosion != null)
                {
                    GameObject explosionInstance = Instantiate(explosion, col.gameObject.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                    Destroy(explosionInstance, 2);
                }                
                col.gameObject.GetComponent<EnemyAttributes>().DealDamage(damage);
                cooldownTimer = 0;
            }
        }
    }

    bool validTarget(GameObject potentialTarget)
    {
        if (potentialTarget.tag == "Enemy" &&
            potentialTarget.GetComponent<EnemyAttributes>().GetIsDead() == false &&
            (potentialTarget.transform.position - transform.position).magnitude < range)
        {
            return true;
        }
        return false;
    }
}
