using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainBolt : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject staticArrowPrefab;
    public GameObject secondaryArrowPrefab;
    public int baseDamage;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player")
        {
            GameObject gb;
            if (collision.collider.tag == "Map")
                gb = Instantiate(staticArrowPrefab, transform.position, transform.rotation);
            else
                gb = Instantiate(staticArrowPrefab, transform.position, transform.rotation, collision.transform);

            gb.transform.Translate(gb.transform.forward * 0.8f);
            Destroy(gb.gameObject, 10f);            // destroy spawned arrow after 10 secs

            // damage if its an enemy
            if (collision.collider.tag == "Enemy")
            {
                collision.collider.GetComponent<EnemyAttributes>().DealDamage(baseDamage);
                ChainShot(collision.collider);
            }


            Destroy(this.gameObject);                // Destroy Rigid body arrow.
        }
    }

    void ChainShot(Collider col)
    {
        //Debug.Log("Called Chainshot");
        Collider[] arr = Physics.OverlapSphere(col.transform.position,15f);

        int count=0;
        Vector3 spawnPos=col.transform.position;
        spawnPos.y += 2.3f;                      // eventually replace with height
        foreach (Collider c in arr) 
        {
            if (c.tag == "Enemy" && !c.GetComponent<EnemyAttributes>().isDead && c!=col)
            {
                count++;
                //Debug.Log("Spawned "+count);

                GameObject gb = Instantiate(secondaryArrowPrefab, spawnPos, Quaternion.identity);
                Vector3 aimAt = c.transform.position;
                aimAt.y += 2f;
                gb.transform.LookAt(aimAt);
                //gb.GetComponent<Rigidbody>().isKinematic = true;
                gb.GetComponent<Rigidbody>().AddForce(gb.transform.forward*25f,ForceMode.Impulse);
                gb.GetComponent<Arrow>().SetArrowDamage(6);
                spawnPos.y += 0.18f;
                //Debug.Log(c.transform.name);

                if (count > 10)
                    break;
            }
        }
        Debug.Log(count+" enemies found around this target.");
    }
}
