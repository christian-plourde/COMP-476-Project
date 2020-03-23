using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//the list of actions the player can make
public enum PlayerAction { ATTACK, RUN, BUILD }

/// <summary>
/// This is a cell in the ngram. Each one represents some action so for example ATTACK, RUN, RUN, and tracks the number of 
/// occurences in the data and allows us to get probability.
/// </summary>
public class NGramCell
{
    private List<PlayerAction> actions; //the actions this cell represents
    private int occurences; //the number of occurences of the next thing after the string to match
    private int samples; //the number of times we matched the strring when processing training data.
    private int id; //to uniquely identify the cell when searching
    private static int index = 0; //static incremented every time to make sure that the id is unique

    public int Samples
    {
        get { return samples; }
    }

    public double Probability
    {
        get { return (double)occurences / (double)samples; }
    }

    //checks if this cell has the same action header as the input string passed in. Necessary for finding the cell
    //matching the input string we are using to compare probabilities
    public bool IsEqual(List<PlayerAction> input_string)
    {
        bool sameness = true;
        for(int i = 0; i < actions.Count; i++)
        {
            if(actions[i] != input_string[i])
            {
                sameness = false;
                break;
            }
        }

        return sameness;
    }

    public NGramCell()
    {
        this.id = NGramCell.index++;
        actions = new List<PlayerAction>();
    }

    //add a character action to the list of actions that this cell represents. Used when building the cell initially
    public void AppendCharacterAction(PlayerAction action)
    {
        actions.Add(action);
    }

    public override string ToString()
    {
        string s = "[";

        if (actions.Count == 0)
            return "[]";

        if (actions.Count == 1)
            return "[" + actions[0].ToString() + "]";
        
        for(int i = 0; i < actions.Count; i++)
        {
            if (i == (actions.Count - 1))
                s += actions[i].ToString();

            else
                s += actions[i].ToString() + ", ";
        }

        s += "]";
        return s;
    }

    //used whenever an occurence of this string (or observation) is found. First we check if the observation is the same (the first
    //actions minus the last one. If it is add a sample. Then check if the entire strings are the same. If so then add an occurence
    public void UpdateCell(List<PlayerAction> temp)
    {
        //check if observation is the same
        bool sameness = true;

        for(int i = 0; i < temp.Count - 1; i++)
        {
            if(temp[i] != actions[i])
            {
                sameness = false;
                break;
            }
        }

        if (sameness)
            samples++;

        //check if the strings are the same
        sameness = true;
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i] != actions[i])
            {
                sameness = false;
                break;
            }
        }

        if (sameness)
            occurences++;
    }

}

//The actual ngram is a collection of cells with prediction capability
public class NGram
{
    private int level; //the level of the ngram. level = 1 means a 1 gram, etc.
    NGramManager manager; //a reference to the manager, which contains the training data to be parsed.
    NGramCell[] cells; //the cells contained in this ngram

    //used to predict the next action
    public PlayerAction Predict()
    {
        //first create the input string
        //input string based on prediction depth in ngrammanager

        List<PlayerAction> observation = manager.Data.GetLastObservations(level - 1); //observation length is the level - 1. So if
        //we have a 3 gram we look at the last two inputs and try to predict the third.

        double best_probability = 0; //keep track of the best probability so far
        PlayerAction predicted_action = PlayerAction.ATTACK; //this will keep track of which action had the best probability
        int failed_samples = 0; //keep track of if the ngram did not contain enough samples to predict with

        for(int i = 0; i < Enum.GetNames(typeof(PlayerAction)).Length; i++) //for each player action
        {
            List<PlayerAction> input_string = new List<PlayerAction>(); //this is the input string to test. It is the observation
            //plus each combination of possible actions

            for(int j = 0; j < observation.Count; j++)
            {
                input_string.Add(observation[j]);
            }

            input_string.Add((PlayerAction)i);

            /*
            string s = string.Empty;

            foreach(PlayerAction p in input_string)
            {
                s += p.ToString() + " ";
            }
            */

            //Debug.Log(s);

            foreach(NGramCell n in cells) //for each cell
            {
                if(n.IsEqual(input_string)) //check if the cell is equal to the input string
                {
                    //if we dont have enough samples increment the counter of failures and break
                    if (n.Samples < manager.SamplesForPrediction)
                    {
                        failed_samples++;
                        break;
                    }
                    
                    //if we have enough samples we can update the best probability if appropriate
                    else
                    {
                        //Debug.Log(n.Probability + " " + best_probability);
                        if (n.Probability > best_probability)
                        {
                            best_probability = n.Probability;
                            predicted_action = (PlayerAction)i;
                        }
                    }

                    break;
                }
            }
        }

        //if we had too many cases where we failed sample check (i.e. each action had not enough samples, we throw an exception
        //this is caught in calling method and causes us to descenf in ngram heirarchy
        if (failed_samples == Enum.GetNames(typeof(PlayerAction)).Length)
        {
            //Debug.Log("not enough samples");
            throw new Exception();
        }
            
        //if we had enough samples we return the predicted action
        return predicted_action;
    }

    public NGram(NGramManager manager, int level)
    {
        this.manager = manager;
        this.level = level;
        cells = new NGramCell[(int)Math.Pow(Enum.GetValues(typeof(PlayerAction)).Length, level)];

        for(int i = 0; i < cells.Length; i++)
        {
            cells[i] = new NGramCell();
        }

        //now we need to set the actions for each of the cells

        for(int i = 0; i < level; i++)
        {
            int action_index = 0;

            for (int j = 0; j < cells.Length; j++)
            {
                
                if(j != 0 && j%((int)Math.Pow(Enum.GetValues(typeof(PlayerAction)).Length, level - (i + 1))) == 0)
                    action_index = (action_index + 1) % (Enum.GetValues(typeof(PlayerAction)).Length);
                cells[j].AppendCharacterAction((PlayerAction)action_index);
            }
        }
    }

    public override string ToString()
    {
        string s = string.Empty;

        foreach(NGramCell c in cells)
        {
            s += c.ToString() + "\n";
        }

        return s;
    }

    //this will recalculate the probabilities etc. for n gram
    public void Update()
    {
        for(int i = 0; i < cells.Length; i++)
        {
            //for each cell (each combination) parse the string and fill in probabilities
            for(int j = 0; j < manager.Data.Length; j++)
            {
                try
                {
                    List<PlayerAction> temp = new List<PlayerAction>();

                    for(int k = j; k < j + level; k++)
                    {
                        temp.Add(manager.Data[k]);
                    }

                    //compare temp string with cell header
                    cells[i].UpdateCell(temp);
                }

                catch { continue; }
            }
        }
    }
}

/// <summary>
/// Circular list that holds a set number of data to test with
/// </summary>
public class NGramString
{
    private int capacity; //the max count of data in the list
    private int length; //the current length of the list
    private NGramStringNode front; //the front of the list
    private NGramStringNode back; //the back of the list

    //this will get the last observations from the list. It will give (for observation count 2 for example) a list containing 2
    //observations to be used in the ngrams predict function
    public List<PlayerAction> GetLastObservations(int observation_count)
    {
        List<PlayerAction> toReturn = new List<PlayerAction>();

        if(observation_count == 1)
        {
            toReturn.Add(front.Value);
            return toReturn;
        }

        else
        {
            toReturn.Add(front.Value);
            NGramStringNode curr = back;
            for (int i = 1; i < observation_count; i++)
            {
                toReturn.Add(curr.Value);
                curr = curr.Previous;
            }
            toReturn.Reverse();
            return toReturn;
        }
    }

    //index operator for easy access to elements in the string
    public PlayerAction this[int index]
    {
        get {

            int i = 0;
            NGramStringNode curr = front;

            while(curr != null)
            {
                if (i == index)
                    return curr.Value;
                i++;
                curr = curr.Next;
            }

            throw new Exception();
        
        }
    }

    public int Length
    {
        get { return length; }
    }

    public NGramString(int capacity)
    {
        this.capacity = capacity;
        this.length = 0;
        this.front = null;
        this.back = null;
    }

    public override string ToString()
    {
        NGramStringNode curr = front;

        if (length == 0)
            return "[]";

        if (length == 1)
            return "[" + front.Value.ToString() + "]";

        string s = "[";

        while(curr != null)
        {
            if (curr.Next == null)
                s += curr.Value.ToString();

            else
                s += curr.Value.ToString() + ", ";

            curr = curr.Next;
        }

        s += "]";

        return s;
    }

    //circular addition to the list. When it's full, we add to the front and delete the last element in the list
    public void Add(PlayerAction action)
    {
        //this is to add a player action to the string
        if(length == 0)
        {
            //if the list is empty, simply create a new node, make it equal to the front and the back
            //increase length by 1
            NGramStringNode n = new NGramStringNode(action);
            front = n;
            back = n;
            back.Previous = front;
            n.Next = back;
            length++;
        }

        else if(length < capacity)
        {
            //if the length is less than the capacity then we just add to the back
            //increase length by 1
            back.Next = new NGramStringNode(action);
            back.Next.Previous = back;
            back = back.Next;
            length++;
        }

        else
        {
            NGramStringNode n = new NGramStringNode(action);
            n.Next = front;
            front.Previous = n;
            front = n;
            //trim the last node off
            back = back.Previous;
            back.Next = null;
        }
    }
}

//type used in ngram string. has pointers to next and previous and holds a player action as its value
public class NGramStringNode
{
    private NGramStringNode next;
    private NGramStringNode previous;
    private PlayerAction action;
    
    public PlayerAction Value
    {
        get { return action; }
    }

    public NGramStringNode Next
    {
        get { return next; }
        set { next = value; }
    }

    public NGramStringNode Previous
    {
        get { return previous; }
        set { previous = value; }
    }

    public NGramStringNode(PlayerAction action)
    {
        this.action = action;
    }
}

//holds a set of ngrams. this is the class to be used by user.
public class NGramManager
{
    List<NGram> ngrams; //heirarchy of ngrams
    NGramString player_actions; //the training data
    int depth_of_prediction; //the depth of prediction. If three, we create a 3 gram heirarchy and when predicting we look
    //at last two observations to guess the third
    int samples_for_prediction; //the number of samples required for prediction by ngram
    private PlayerBuffManager player_buffs; //the player buffs

    public int SamplesForPrediction
    {
        get { return samples_for_prediction; }
    }

    public NGramString Data
    {
        get { return player_actions; }
    }

    /// <summary>
    /// This creates a heirarchical n gram.
    /// </summary>
    /// <param name="string_capacity">The size of the live circular training data set</param>
    /// <param name="depth_of_prediction">The number of samples it looks at for predicting (the input string length)</param>
    /// <param name="samples_for_prediction">The number of samples required for prediction. Should be larger than 0.</param>
    public NGramManager(int string_capacity, int depth_of_prediction, int samples_for_prediction, PlayerBuffManager player_buffs)
    {
        this.player_buffs = player_buffs;
        ngrams = new List<NGram>();
        player_actions = new NGramString(string_capacity);
        this.depth_of_prediction = depth_of_prediction;
        if (samples_for_prediction <= 0)
            this.samples_for_prediction = 1;
        else
            this.samples_for_prediction = samples_for_prediction;

        //the depth of prediction determines how many n gram levels we want.
        //so if depth of prediction is 4 for example we will go up to a 4 gram
        for(int i = 0; i < depth_of_prediction; i++)
        {
            AddNGram(new NGram(this, i + 1));
        }
    }

    //if ngram predict fails we move to the next ngram, in descending order since they are in increasing order of
    //complxity in ouyr collection
    public List<Buff> Predict()
    {
        int i = ngrams.Count - 1;

        while(i >= 0)
        {
            try
            {
                string res = ngrams[i].Predict().ToString();

                List<Buff> toReturn = new List<Buff>();
                foreach(Buff b in player_buffs.Buffs)
                {
                    if(b.category == res)
                    {
                        toReturn.Add(b);
                    }
                }

                return toReturn;

            }

            catch
            {
                i--;
            }
        }

        throw new Exception();
    }

    private void AddNGram(NGram nGram)
    {
        this.ngrams.Add(nGram);
    }

    //used to add a player action to the list.
    //every time this is done, the samples and occurences are recalculated for each of the ngrams we have
    public void AppendPlayerAction(PlayerAction action)
    {
        player_actions.Add(action);

        //we add an action to the string and then update the n grams
        foreach(NGram n in ngrams)
        {
            n.Update();
        }
    }

    public override string ToString()
    {
        string s = string.Empty;
        foreach(NGram n in ngrams)
        {
            s += n.ToString() + "\n";
        }

        return s;
    }
}