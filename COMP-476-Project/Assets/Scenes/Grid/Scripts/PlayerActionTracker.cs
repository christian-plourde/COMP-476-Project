using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionTracker : Observer
{
    NGramManager n_gram;
    [Header("Action Timers")]
    [Tooltip("The number of seconds the player must run for the ngram to register a run action")]
    public float runTimerSeconds = 1.0f; //how many seconds player must run for it to count as a run action for ngram
    [Tooltip("The number of attack the player must make for the ngram to register an attack action")]
    public int attackCount = 6; //how many hits player must do to count as an attack in n gram
    [Tooltip("UI Prefab")]
    public GameObject UIPrefab;
    public GameObject Canvas;

    private float runTimer = 0.0f; //the current run timer (how long the player has been running). only reset when it exceeds
                                   //threshold for adding an action to training data

    private int attackCounter = 0; //the current attack count (how many times the player has attacked). only reset when it exceeds
                                   //threshold for adding an action to training data.

    private List<Buff> available_buffs; //the predicted action at the end of the wave. determines which buffs player has access to

    private PlayerBuffManager player_buffs; //all of the buffs available to player



    // Start is called before the first frame update
    void Start()
    {
        player_buffs = FindObjectOfType<PlayerBuffManager>();
        n_gram = new NGramManager(100, 3, 2, player_buffs); //set ngram manager to get our heirarchy
        //Debug.Log(n_gram);
        
    }

    // Update is called once per frame
    void Update()
    {
        //if player pressed a movement key, increment the run timer by frame time
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            runTimer += Time.deltaTime;
        }

        //if player has attacked or done secondary attack, increment the attack count
        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) && !FindObjectOfType<PlayerMovement>().inBuildMode)
        {
            attackCounter++;
        }

        //if player has done ultimate ability increment attak counter by attack count limit
        if((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.T)) && !FindObjectOfType<PlayerMovement>().inBuildMode)
        {
            attackCounter += attackCount;
        }

        //now we need to check if we should append a run or attack action
        //first for run
        if(runTimer >= runTimerSeconds)
        {
            n_gram.AppendPlayerAction(PlayerAction.RUN);
            runTimer -= runTimerSeconds;
        }

        //then for attack
        if(attackCounter >= attackCount)
        {
            n_gram.AppendPlayerAction(PlayerAction.ATTACK);
            attackCounter -= attackCount;
        }
    }

    public void SetBuffCategory()
    {
        try
        {
            //set the buff category using the n grams prediction.
            available_buffs = n_gram.Predict();
            //Debug.Log(buff_category);
            //
            //instantiate the UI prefab where the player can select which buff he'd like to get.
            GameObject temp = Instantiate(UIPrefab, UIPrefab.transform.position, Quaternion.identity);
            temp.transform.SetParent(Canvas.transform, false);
            temp.GetComponent<BuffUIScript>().BList = available_buffs;

        }

        catch
        {
            Debug.Log("NGram failed to predict next action.");
        }
    }

    //attached to the subject generate grid. This is called whenever a tower is placed and allows us to add a build action to
    //ngram data
    public override void ObserverUpdate()
    {
        n_gram.AppendPlayerAction(PlayerAction.BUILD);
    }
}
