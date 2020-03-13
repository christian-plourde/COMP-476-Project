using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWolf : MonoBehaviour
{
    float timer;

    void Start()
    {
        GetComponent<Animator>().SetBool("Chasing",true);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 9)
            Destroy(this.gameObject);
        transform.Translate(transform.forward*11f*Time.deltaTime);   

    }
}
