using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionTracker : Observer
{
    NGramManager n_gram;

    // Start is called before the first frame update
    void Start()
    {
        n_gram = new NGramManager(30, 3, 2);
        //Debug.Log(n_gram);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            n_gram.AppendPlayerAction(PlayerAction.RUN);

        if (Input.GetKey(KeyCode.Mouse0) && !FindObjectOfType<PlayerMovement>().inBuildMode)
            n_gram.AppendPlayerAction(PlayerAction.ATTACK);

        if(Input.GetKey(KeyCode.O))
        {
            try
            {
                //Debug.Log(n_gram.Data);
                Debug.Log(n_gram.Predict());
            }

            catch
            {

            }
        }
    }

    public override void ObserverUpdate()
    {
        n_gram.AppendPlayerAction(PlayerAction.BUILD);
    }
}
