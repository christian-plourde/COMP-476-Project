using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionTracker : Observer
{
    NGramManager n_gram;
    [Header("Action Timers")]
    public int runTimerSeconds = 10; //how many seconds player must run for it to count as a run action for ngram
    public int attackTimerSeconds = 10; //how many seconds player needs to attack for it to count as a run action for ngram

    private float runTimer = 0.0f; //the current run timer (how long the player has been running). only reset when it exceeds
                                   //threshold for adding an action to training data

    private float attackTimer = 0.0f; //the current attack timer (how long the player has been attacking). only reset when it exceeds
                                      //threshold for adding an action to training data.

    // Start is called before the first frame update
    void Start()
    {
        n_gram = new NGramManager(30, 3, 2); //set ngram manager to get our heirarchy
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

    //attached to the subject generate grid. This is called whenever a tower is placed and allows us to add a build action to
    //ngram data
    public override void ObserverUpdate()
    {
        n_gram.AppendPlayerAction(PlayerAction.BUILD);
    }
}
