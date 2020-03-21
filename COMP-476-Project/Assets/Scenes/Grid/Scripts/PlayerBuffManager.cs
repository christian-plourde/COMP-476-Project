using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class BuffMethods
{
    public BuffMethods()
    {

    }

    public void BuffTowerDamage(Buff buff)
    {
        Debug.Log("Buffing Tower Damage");
    }

    public void BuffAttackDamage(Buff buff)
    {
        Debug.Log("Buffing Player Damage");
    }

    public void BuffPlayerSpeed(Buff buff)
    {
        Debug.Log("Buffing Player Speed");
    }

    public void BuffRallyingCall(Buff buff)
    {
        Debug.Log("Buffing Rallying Call");
    }

    public void BuffCementSoup(Buff buff)
    {
        Debug.Log("Buffing Cement Soup");
    }

    public void BuffArtOfWar(Buff buff)
    {
        Debug.Log("Buffing Art of War");
    }

    public void BuffFastestManAlive(Buff buff)
    {
        Debug.Log("Buffing Fastest Man Alive");
    }

    public void BuffThriller(Buff buff)
    {
        Debug.Log("Buffing Thriller");
    }

    public void BuffDivideAndConquer(Buff buff)
    {
        Debug.Log("Buffing Divide and Conquer");
    }
}

[System.Serializable]
public class Buff
{
    public int uid; //a unique identifier for the buff specified in json file
    public string name; //the name of the buff that the player will see (json file)
    public string description; //a description of what the buff does (json file)
    public string category; //the category of the buff (ATTACK, RUN, or BUILD)
    public int[] prerequisites; //a list of uid's that must be unlocked before this buff can be leveled up
    public string method; //the name of the method to be executed when buff is active
    
    [System.NonSerialized]
    private List<Buff> buff_dependencies; //the list of buffs that this buff needs before unlocking. derived from the list of uid's
                                          //at run time
    
    [System.NonSerialized]
    private BuffList buffList; //a reference to the list of all buffs. used to set the prereq buffs

    [System.NonSerialized]
    private BuffMethods buff_methods;

    [System.NonSerialized]
    private int level; //the level of this buff

    public BuffMethods BuffMethods
    {
        set { buff_methods = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public bool CanUpgrade()
    {
        foreach(Buff b in buff_dependencies)
        {
            if(b.level <= level)
            {
                return false;
            }
        }

        return true;
    }

    public void ApplyBuff()
    {
        /*
        if (level == 0)
            return;
            */

        System.Type bm_type = buff_methods.GetType();
        
        MethodInfo fn = bm_type.GetMethod(this.method);
        object[] param = { this };
        fn.Invoke(this.buff_methods, param);
    }

    public Buff()
    {
        buff_dependencies = new List<Buff>();
        level = 0;
    }

    public BuffList BuffList
    {
        set { buffList = value; }
    }
    
    public void SetPrerequisites()
    {
        foreach(int i in prerequisites)
        {
            foreach(Buff b in buffList.buffs)
            {
                if(b.uid == i)
                {
                    buff_dependencies.Add(b);
                    break;
                }
            }
        }
    }

    public override string ToString()
    {
        return name;
    }
}

[System.Serializable]
public class BuffList
{
    public Buff[] buffs;

    public override string ToString()
    {
        string s = string.Empty;

        s += "[";

        for(int i = 0; i < buffs.Length; i++)
        {
            if (i == (buffs.Length - 1))
                s += buffs[i];

            else
                s += buffs[i] + ", ";
        }

        s += "]";

        return s;
    }
}

//responsible for loading in the player buffs as well as setting the player buffs each frame (the ones he has unlocked)
public class PlayerBuffManager : MonoBehaviour
{
    [Header("Player Buff Definitions")]
    public TextAsset player_buffs_json;

    private BuffList buff_list;

    // Start is called before the first frame update
    void Start()
    {
        buff_list = JsonUtility.FromJson<BuffList>(player_buffs_json.text); //load all buffs from json

        foreach(Buff b in buff_list.buffs)
        {
            b.BuffList = buff_list; //set buff list reference for each buff
            b.BuffMethods = new BuffMethods();
            b.SetPrerequisites(); //set prerequisites for each buff from the buff list by uid
            b.ApplyBuff();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
