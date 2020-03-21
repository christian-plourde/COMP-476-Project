using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class BuffMethods
{
    private float rallying_call_effect_range;

    public float RallyingCallEffectRange
    {
        get { return rallying_call_effect_range; }
        set { rallying_call_effect_range = value; }
    }

    public BuffMethods()
    {

    }

    public void BuffTowerDamage(Buff buff)
    {
        float inc_per_level = 0.0f;

        try
        {
            foreach(TowerAttack t in GameObject.FindObjectsOfType<TowerAttack>())
            {
                //Debug.Log("Found Tower " + t.name);
                t.damage = (1 + buff.Level * inc_per_level) * t.damage; //adds 10 percent damage per level to each tower
                //Debug.Log("Damage is now: " + t.damage);
            }
        }

        catch { }
    }

    public void BuffAttackDamage(Buff buff)
    {
        float inc_per_level = 0.05f;

        //Debug.Log("Buffing Player Damage");
        //first we need to find either the warrior combat behaviour or the archer combat behaviour
        //lets try warrior first
        try
        {
            WarriorCombatBehavior wc = GameObject.FindObjectOfType<WarriorCombatBehavior>();
            //Debug.Log("old damage: " + wc.HandSword.GetComponent<BastardSword>().baseDMG);
            wc.BaseDamage = (1 + buff.Level * inc_per_level) * wc.BaseDamage; //increase warrior base damage by a factor of 5 percent
            //Debug.Log("new damage: " + wc.HandSword.GetComponent<BastardSword>().baseDMG);
        }

        catch
        {

        }

        //now let's try the archer
        try
        {
            CombatBehavior ac = GameObject.FindObjectOfType<CombatBehavior>();
            ac.baseDamage = (1 + buff.Level * inc_per_level) * ac.baseDamage;
        }

        catch
        {

        }
    }

    public void BuffPlayerSpeed(Buff buff)
    {
        float inc_per_level = 0.02f;

        //Debug.Log("Buffing Player Speed");
        try
        {
            PlayerMovement m = GameObject.FindObjectOfType<PlayerMovement>();
            m.mSpeed = (1 + inc_per_level * buff.Level) * m.mSpeed;
        }

        catch
        {

        }
    }

    public void BuffRallyingCall(Buff buff)
    {
        float inc_per_level = 0.02f;

        //Debug.Log("Buffing Rallying Call");
        //increases tower attack speed when player is nearby
        PlayerMovement p = GameObject.FindObjectOfType<PlayerMovement>();

        foreach(TowerAttack t in GameObject.FindObjectsOfType<TowerAttack>())
        {
            if ((p.transform.position - t.transform.position).magnitude < RallyingCallEffectRange)
            {
                t.RallyingCallMultiplier = (1 - inc_per_level * buff.Level) * t.RallyingCallMultiplier;
            }

            else
            {
                t.RallyingCallMultiplier = 1;
            }
        }
    }

    public void BuffCementSoup(Buff buff)
    {
        //Debug.Log("Buffing Cement Soup");
    }

    public void BuffArtOfWar(Buff buff)
    {
        //Debug.Log("Buffing Art of War");
    }

    public void BuffFastestManAlive(Buff buff)
    {
        //Debug.Log("Buffing Fastest Man Alive");
    }

    public void BuffThriller(Buff buff)
    {
        //Debug.Log("Buffing Thriller");
    }

    public void BuffDivideAndConquer(Buff buff)
    {
        //Debug.Log("Buffing Divide and Conquer");
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
        get { return buff_methods; }
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

    [Header("Rallying Call Parameters")]
    public float rallyingCallEffectRange = 0;

    private BuffList buff_list;

    // Start is called before the first frame update
    void Start()
    {
        buff_list = JsonUtility.FromJson<BuffList>(player_buffs_json.text); //load all buffs from json

        foreach(Buff b in buff_list.buffs)
        {
            b.BuffList = buff_list; //set buff list reference for each buff
            b.BuffMethods = new BuffMethods();
            b.BuffMethods.RallyingCallEffectRange = this.rallyingCallEffectRange;
            b.SetPrerequisites(); //set prerequisites for each buff from the buff list by uid
            //b.ApplyBuff();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            foreach(Buff b in buff_list.buffs)
            {
                b.Level++;
                b.ApplyBuff();
            }
        }
    }
}
