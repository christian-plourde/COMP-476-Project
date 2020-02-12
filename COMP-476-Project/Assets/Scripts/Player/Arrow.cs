using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Instantiate an arrow at point of impact
    // call delete on it in 120 seconds

    public GameObject arrowPrefab;

    float Timer;

    private void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > 120)
            Destroy(this.gameObject);             // just incase arrow doesnt collide with anything
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player")
        {
            GameObject gb = Instantiate(arrowPrefab, transform.position, transform.rotation,collision.transform);
            gb.transform.Translate(gb.transform.forward * 0.4f);
            Debug.Log("Hit Object " + collision.transform.name);
            Destroy(gb.gameObject, 120f);            // destroy spawned arrow after 120 secs
            Destroy(this.gameObject);                // Destroy Rigid body arrow.
        }
    }
}
