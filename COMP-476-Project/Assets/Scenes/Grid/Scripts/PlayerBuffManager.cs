using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class BuffMethods
{
    private float rallying_call_effect_range;
    private float fastest_man_alive_charge_timer_time = 0; //current time moving (progress to fastest man alive)
    private float fastest_man_alive_charge_timer_length = 15; //move time required for fastest man alive
    private bool fastest_man_alive_charged = false; //boolean to say if fastest man alive is currently active
    private float fastest_man_alive_discharge_timer_time = 0; //current time elapsed in fastest man alive ability (when active)
    private float fastest_man_alive_discharge_timer_length = 5; //how long fastest man alive ability stays active once it is enabled
    private float fastest_man_alive_active_multiplier = 2; //multiplier for damage while fastest man alive is active
    private bool towerPlaced = false; //indicates that tower was placed by player so that we can start move speed boost timer in
                                      //the divide and conquer method
    private float divide_and_conquer_timer_time = 0; //the time elapsed since divide and conquer ability was activated
    private float divide_and_conquer_timer_length = 5; //the time for which divide and conquer ability stays active when activated

    public float FastestManAliveChargeProgress
    {
        get { return fastest_man_alive_charge_timer_time / fastest_man_alive_charge_timer_length; }
    }

    public bool TowerPlaced
    {
        get { return towerPlaced; }
        set { towerPlaced = value; }
    }

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
        float inc_per_level = 0.02f;
        buff.Active = true;

        try
        {
            foreach(TowerAttack t in GameObject.FindObjectsOfType<TowerAttack>())
            {
                //Debug.Log("Found Tower " + t.name);
                t.damage = (1 + buff.Level * inc_per_level) * t.damage; //adds 2 percent damage per level to each tower
                //Debug.Log("Damage is now: " + t.damage);
            }
        }

        catch { }
    }

    public void BuffAttackDamage(Buff buff)
    {
        float inc_per_level = 0.05f;
        buff.Active = true;

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
        buff.Active = true;

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
                buff.Active = true;
                t.RallyingCallMultiplier = (1 - inc_per_level * buff.Level);
            }

            else
            {
                buff.Active = false;
                t.RallyingCallMultiplier = 1;
            }
        }
    }

    public void BuffCementSoup(Buff buff)
    {
        //Debug.Log("Buffing Cement Soup");
        //this will increase the health of the tower by set amount per level each time an enemy dies
        float inc_per_level = 5f;
        buff.Active = true;

        foreach (BuildingStats stats in GameObject.FindObjectsOfType<BuildingStats>())
        {
            stats.health = Mathf.Min(stats.MaxHealth, (5 * buff.Level) + stats.health);
            //Debug.Log(stats.health);
        }
    }

    public void BuffArtOfWar(Buff buff)
    {
        //Debug.Log("Buffing Art of War");
        //player damage is proportional to the number of towers in play
        float inc_per_level = 0.02f;

        try
        {
            if (GameObject.FindObjectsOfType<BuildingStats>().Length > 0)
                buff.Active = true;
            else
                buff.Active = false;
        }

        catch
        {

        }
        

        //lets try warrior first
        try
        {
            WarriorCombatBehavior wc = GameObject.FindObjectOfType<WarriorCombatBehavior>();
            //Debug.Log("old damage: " + wc.HandSword.GetComponent<BastardSword>().baseDMG);
            wc.ArtOfWarMultiplier = (1 + buff.Level * inc_per_level * GameObject.FindObjectsOfType<BuildingStats>().Length);
            //Debug.Log("new damage: " + wc.HandSword.GetComponent<BastardSword>().baseDMG);
        }

        catch
        {

        }

        //now let's try the archer
        try
        {
            CombatBehavior ac = GameObject.FindObjectOfType<CombatBehavior>();
            ac.ArtOfWarMultiplier = (1 + buff.Level * inc_per_level * GameObject.FindObjectsOfType<BuildingStats>().Length);
            //Debug.Log(ac.baseDamage * ac.ArtOfWarMultiplier);
        }

        catch
        {

        }
    }

    public void BuffFastestManAlive(Buff buff)
    {
        //Debug.Log("Buffing Fastest Man Alive");
        //charge a timer until full. when full it activates and player gets a damage boost for limited amount of time
        float inc_per_level = 0.1f;

        //lets charge the timer for the ability
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && buff.Level > 0 && !fastest_man_alive_charged)
        {
            //Debug.Log("Charging timer");
            fastest_man_alive_charge_timer_time += Time.deltaTime * (1 + inc_per_level * buff.Level);
            buff.Active = false;
        }

        if(fastest_man_alive_charge_timer_time >= fastest_man_alive_charge_timer_length)
        {
            fastest_man_alive_charge_timer_time = 0;
            fastest_man_alive_charged = true;
            fastest_man_alive_discharge_timer_time = 0;
            //Debug.Log("Timer is charged");
            buff.Active = true;
        }

        if(fastest_man_alive_charged && fastest_man_alive_discharge_timer_time <= fastest_man_alive_discharge_timer_length)
        {
            buff.Active = true;
            fastest_man_alive_discharge_timer_time += Time.deltaTime;
            //Debug.Log("Timer discharging");
            //lets try warrior first
            try
            {
                WarriorCombatBehavior wc = GameObject.FindObjectOfType<WarriorCombatBehavior>();
                //Debug.Log("old damage: " + wc.HandSword.GetComponent<BastardSword>().baseDMG);
                wc.FastestManAliveMultiplier = (fastest_man_alive_active_multiplier);
                //Debug.Log("new damage: " + wc.HandSword.GetComponent<BastardSword>().baseDMG);
                //Debug.Log(wc.baseDamage * wc.ArtOfWarMultiplier * wc.FastestManAliveMultiplier);
            }

            catch
            {

            }

            //now let's try the archer
            try
            {
                CombatBehavior ac = GameObject.FindObjectOfType<CombatBehavior>();
                ac.FastestManAliveMultiplier = (fastest_man_alive_active_multiplier);
                //Debug.Log(ac.baseDamage * ac.ArtOfWarMultiplier);
                //Debug.Log(ac.baseDamage * ac.ArtOfWarMultiplier * ac.FastestManAliveMultiplier);
            }

            catch
            {

            }

        }

        if(fastest_man_alive_charged && fastest_man_alive_discharge_timer_time > fastest_man_alive_discharge_timer_length)
        {
            fastest_man_alive_discharge_timer_time = 0;
            fastest_man_alive_charged = false;
            buff.Active = false;
            //Debug.Log("Timer discharged");
            //lets try warrior first
            try
            {
                WarriorCombatBehavior wc = GameObject.FindObjectOfType<WarriorCombatBehavior>();
                //Debug.Log("old damage: " + wc.HandSword.GetComponent<BastardSword>().baseDMG);
                wc.FastestManAliveMultiplier = (1);
                //Debug.Log("new damage: " + wc.HandSword.GetComponent<BastardSword>().baseDMG);
                //Debug.Log(wc.baseDamage * wc.ArtOfWarMultiplier * wc.FastestManAliveMultiplier);
            }

            catch
            {

            }

            //now let's try the archer
            try
            {
                CombatBehavior ac = GameObject.FindObjectOfType<CombatBehavior>();
                ac.FastestManAliveMultiplier = (1);
                //Debug.Log(ac.baseDamage * ac.ArtOfWarMultiplier);
                //Debug.Log(ac.baseDamage * ac.ArtOfWarMultiplier * ac.FastestManAliveMultiplier);
            }

            catch
            {

            }
        }
    }

    public void BuffThriller(Buff buff)
    {
        //Debug.Log("Buffing Thriller");
        //Debug.Log(GameObject.FindObjectOfType<PlayerMovement>().mSpeed * GameObject.FindObjectOfType<PlayerMovement>().ThrillerMultiplier);
        float inc_per_level = 0.05f;

        bool being_chased = false;

        foreach(Character c in GameObject.FindObjectsOfType<Character>())
        {
            if(c.BehaviourType == BEHAVIOUR_TYPE.ATTACK_PLAYER)
            {
                being_chased = true;
            }
        }

        buff.Active = being_chased;

        try
        {
            if (being_chased)
                GameObject.FindObjectOfType<PlayerMovement>().ThrillerMultiplier = (1 + inc_per_level * buff.Level);
            else
                GameObject.FindObjectOfType<PlayerMovement>().ThrillerMultiplier = (1);
        }

        catch { }
    }

    public void BuffDivideAndConquer(Buff buff)
    {
        float inc_per_level = 0.05f;
        //Debug.Log("Buffing Divide and Conquer");
        if(TowerPlaced)
        {
            divide_and_conquer_timer_time = 0;
            TowerPlaced = false;
        }

        divide_and_conquer_timer_time += Time.deltaTime;

        if(divide_and_conquer_timer_time >= divide_and_conquer_timer_length)
        {
            GameObject.FindObjectOfType<PlayerMovement>().DivideAndConquerMultiplier = 1;
            buff.Active = false;
        }

        else
        {
            GameObject.FindObjectOfType<PlayerMovement>().DivideAndConquerMultiplier = (1 + inc_per_level * buff.Level);
            buff.Active = true;
        }
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
    public bool repeatable; //indicates if buff is applied every frame or not
    
    [System.NonSerialized]
    private List<Buff> buff_dependencies; //the list of buffs that this buff needs before unlocking. derived from the list of uid's
                                          //at run time
    
    [System.NonSerialized]
    private BuffList buffList; //a reference to the list of all buffs. used to set the prereq buffs

    [System.NonSerialized]
    private BuffMethods buff_methods;

    [System.NonSerialized]
    private int level; //the level of this buff

    [System.NonSerialized]
    private bool active; //indicates if the buff is currently active or not

    public bool Active
    {
        get { return active; }
        set { active = value; }
    }

    public BuffMethods BuffMethods
    {
        get { return buff_methods; }
        set { buff_methods = value; }
    }

    public bool RepeatsEveryFrame
    {
        get { return repeatable; }
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
        System.Type bm_type = buff_methods.GetType();
        
        MethodInfo fn = bm_type.GetMethod(this.method);
        object[] param = { this };
        fn.Invoke(this.buff_methods, param);
    }

    public Buff()
    {
        buff_dependencies = new List<Buff>();
        level = 0;
        active = true;
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

    public string PrerequisitesToString()
    {
        string temp = "";
        foreach (int i in this.prerequisites)
        {
            foreach (Buff b in buffList.buffs)
            {
                if(i == b.uid)
                {
                    temp += b.name + "(" + (this.level+1) + ")";
                    break;
                }
            }
            temp += ", ";
        }
        return temp;
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

    public Buff[] Buffs
    {
        get { return buff_list.buffs; }
    }

    public void ApplyCementSoup()
    {
        foreach(Buff b in buff_list.buffs)
        {
            if(b.method == "BuffCementSoup")
            {
                b.ApplyBuff();
                return;
            }
        }
    }

    public void PlaceTowerCallback()
    {
        foreach(Buff b in buff_list.buffs)
        {
            b.BuffMethods.TowerPlaced = true;
        }
    }

    public void LevelUp(int uid)
    {
        foreach (Buff b in buff_list.buffs)
        {
            if (b.uid == uid)
            {
                b.Level++;
                b.ApplyBuff();
            }
        }
    }

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
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        string s = string.Empty;
        foreach(Buff b in buff_list.buffs)
        {
            if(b.Level > 0 && b.Active)
            {
                s += b.name + " ";
            }
        }

        Debug.Log(s);
        */

        foreach(Buff b in buff_list.buffs)
        {
            if (b.RepeatsEveryFrame)
                b.ApplyBuff();
        }
    }
}
