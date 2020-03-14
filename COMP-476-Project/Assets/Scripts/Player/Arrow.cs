using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Instantiate an arrow at point of impact
    // call delete on it in 120 seconds

    public GameObject arrowPrefab;

    public int baseDamage=5;
    float Timer;

    // for automatic scale adjustment
    float scaleFactor;

    private void Start()
    {
        scaleFactor=GameObject.FindGameObjectWithTag("Player").transform.localScale.x;
        transform.localScale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > 120)
            Destroy(this.gameObject);             // just incase arrow doesnt collide with anything
    }

    public void SetArrowDamage(int dmg)
    {
        baseDamage = dmg;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player")
        {
            GameObject gb;
            if (collision.collider.tag=="Map")
                gb = Instantiate(arrowPrefab, transform.position, transform.rotation);
            else
                gb = Instantiate(arrowPrefab, transform.position, transform.rotation,collision.transform);

            gb.transform.Translate(gb.transform.forward * 0.8f);
            Destroy(gb.gameObject, 10f);            // destroy spawned arrow after 60 secs

            // for scale adjustment on static arrows
            gb.transform.localScale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
            //gb.transform.localScale = new Vector3(gb.transform.localScale.x*0.25f, gb.transform.localScale.x * 0.25f, gb.transform.localScale.x * 0.25f);

            // damage if its an enemy
            if (collision.collider.tag == "Enemy")
            {
                collision.collider.GetComponent<EnemyAttributes>().DealDamage(baseDamage);
            }


            Destroy(this.gameObject);                // Destroy Rigid body arrow.
        }
    }
}
