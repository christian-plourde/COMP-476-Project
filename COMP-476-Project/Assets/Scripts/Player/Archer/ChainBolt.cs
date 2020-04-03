using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainBolt : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject staticArrowPrefab;
    public GameObject secondaryArrowPrefab;
    public int baseDamage;

    // automatic scale adjustment
    float scaleFactor;

    void Start()
    {
        scaleFactor = GameObject.FindGameObjectWithTag("Player").transform.localScale.x;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player")
        {
            /*
            GameObject gb;
            if (collision.collider.tag == "Map")
                gb = Instantiate(staticArrowPrefab, transform.position, transform.rotation);
            else
                gb = Instantiate(staticArrowPrefab, transform.position, transform.rotation, collision.transform);

            gb.transform.Translate(gb.transform.forward * 0.8f);
            Destroy(gb.gameObject, 10f);            // destroy spawned arrow after 10 secs

            // for scale adjustment on static arrows
            gb.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            */

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
        Collider[] arr = Physics.OverlapSphere(col.transform.position,7.5f);

        int count=0;
        Vector3 spawnPos=col.transform.position;
        spawnPos.y += 0.6f;                      // eventually replace with height
        foreach (Collider c in arr) 
        {
            if (c.tag == "Enemy" && !c.GetComponent<EnemyAttributes>().isDead && c!=col)
            {
                count++;
                //Debug.Log("Spawned "+count);

                GameObject gb = Instantiate(secondaryArrowPrefab, spawnPos, Quaternion.identity);
                Vector3 aimAt = c.transform.position;
                aimAt.y += 0.2f;
                gb.transform.LookAt(aimAt);
                //gb.GetComponent<Rigidbody>().isKinematic = true;
                gb.GetComponent<Rigidbody>().AddForce(gb.transform.forward*15.5f,ForceMode.Impulse);
                gb.GetComponent<Arrow>().SetArrowDamage(6);
                spawnPos.y += 0.08f;
                //Debug.Log(c.transform.name);

                if (count > 10)
                    break;
            }
        }
        // Debug.Log(count+" enemies found around this target.");
    }
}
