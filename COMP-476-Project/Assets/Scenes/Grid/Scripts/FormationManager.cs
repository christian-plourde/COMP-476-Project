﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FORMATION_TYPE { INVERTED_4_TRIANGLE, INVERTED_TRIANGLE, TRIANGLE, SIDE_PAIR }

public class FormationManager : MonoBehaviour
{
    public List<GameObject> npcs; //the list of npcs that are in the formation
    private List<Character> characters; //the list of character scripts that are in the formation (initialized in start based on the list of npcs)
    public FORMATION_TYPE formation_type;
    public float formation_scale = 1.0f;
    private Character character; //a reference to the character script on this formation manager

    #region Properties
    public Vector3 Position
    {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }

    public int UnitCount
    {
        get {
            List<Character> toKeep = new List<Character>();
            foreach (Character c in characters)
            {
                if (c == null || c.GetComponent<EnemyAttributes>().isDead)
                {
                    continue;
                }
                toKeep.Add(c);
            }

            characters = toKeep;

            return characters.Count;
        }
    }

    #endregion

    #region Update Formation Setter Functions

    private void SetInverted4Triangle()
    {
        //make sure we have three characters, otherwise inverted triangle formation is impossible.
        if (UnitCount == 4)
        {
            //in order to do this we need to know what is the forward direction of the formation's anchor since one of them will
            //be placed at the back of the formation and the two others to either side.
            
            try
            {
                if (!characters[0].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[0].Immobilized = false;
                    characters[0].Movement.Target = this.Position - 2.0f / 3.0f * formation_scale * this.transform.forward;
                    if (characters[0].transform.position == (this.Position - 2.0f / 3.0f * formation_scale * this.transform.forward))
                        characters[0].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }

            try
            {
                if (!characters[1].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[1].Immobilized = false;
                    characters[1].Movement.Target = this.Position + 1.0f / 3.0f * formation_scale * this.transform.right;
                    if (characters[1].transform.position == (this.Position + 1.0f / 3.0f * formation_scale * this.transform.right))
                        characters[1].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }

            try
            {
                if (!characters[2].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[2].Immobilized = false;
                    characters[2].Movement.Target = this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right;
                    if (characters[2].transform.position == (this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right))
                        characters[2].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }

            try
            {
                if (!characters[3].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[3].Immobilized = false;
                    characters[3].Movement.Target = this.Position + 1.0f / 3.0f * formation_scale * this.transform.forward;
                    if (characters[3].transform.position == (this.Position + 1.0f / 3.0f * formation_scale * this.transform.forward))
                        characters[3].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }
        }

        else if (UnitCount == 3)
            formation_type = FORMATION_TYPE.INVERTED_TRIANGLE;
    }

    private void SetInvertedTriangle()
    {
        //make sure we have three characters, otherwise inverted triangle formation is impossible.
        if (UnitCount == 3)
        {
            //in order to do this we need to know what is the forward direction of the formation's anchor since one of them will
            //be placed at the back of the formation and the two others to either side.

            try
            {
                if (!characters[0].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[0].Immobilized = false;
                    characters[0].Movement.Target = this.Position - 2.0f / 3.0f * formation_scale * this.transform.forward;
                    if (characters[0].transform.position == (this.Position - 2.0f / 3.0f * formation_scale * this.transform.forward))
                        characters[0].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }

            try
            {
                if (!characters[1].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[1].Immobilized = false;
                    characters[1].Movement.Target = this.Position + 1.0f / 3.0f * formation_scale * this.transform.right;
                    if (characters[1].transform.position == (this.Position + 1.0f / 3.0f * formation_scale * this.transform.right))
                        characters[1].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }

            try
            {
                if (!characters[2].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[2].Immobilized = false;
                    characters[2].Movement.Target = this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right;
                    if (characters[2].transform.position == (this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right))
                        characters[2].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }
        }

        else if (UnitCount == 2)
            formation_type = FORMATION_TYPE.SIDE_PAIR;
    }

    private void SetTriangle()
    {
        //make sure we have three characters, otherwise triangle formation is impossible.
        if (UnitCount == 3)
        {
            //in order to do this we need to know what is the forward direction of the formation's anchor since one of them will
            //be placed at the head of the formation and the two others to either side.
            try
            {
                if (!characters[0].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[0].Immobilized = false;
                    characters[0].Movement.Target = this.Position + 2.0f / 3.0f * formation_scale * this.transform.forward;
                    if (characters[0].transform.position == (this.Position + 2.0f / 3.0f * formation_scale * this.transform.forward))
                        characters[0].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }

            try
            {
                if (!characters[1].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[1].Immobilized = false;
                    characters[1].Movement.Target = this.Position + 1.0f / 3.0f * formation_scale * this.transform.right;
                    if (characters[1].transform.position == (this.Position + 1.0f / 3.0f * formation_scale * this.transform.right))
                        characters[1].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }

            try
            {
                if (!characters[2].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[2].Immobilized = false;
                    characters[2].Movement.Target = this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right;
                    if (characters[2].transform.position == (this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right))
                        characters[2].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }
        }

        else if(UnitCount == 2)
            formation_type = FORMATION_TYPE.SIDE_PAIR;

    }

    private void SetSidePair()
    {
        if(UnitCount == 2)
        {
            try
            {
                if (!characters[0].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[0].Immobilized = false;
                    characters[0].Movement.Target = this.Position + 1.0f / 3.0f * formation_scale * this.transform.right;
                    if (characters[0].transform.position == (this.Position + 1.0f / 3.0f * formation_scale * this.transform.right))
                        characters[0].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }

            try
            {
                if (!characters[1].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[1].Immobilized = false;
                    characters[1].Movement.Target = this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right;
                    if (characters[1].transform.position == (this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right))
                        characters[1].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }
        }

        else if (UnitCount == 1)
        {
            try
            {
                if (!characters[0].GetComponent<EnemyAttributes>().isDead)
                {
                    characters[0].Immobilized = false;
                    characters[0].Movement.Target = this.Position;
                    if (characters[0].transform.position == this.Position)
                        characters[0].Movement.Orientation = character.Movement.Orientation;
                }
            }

            catch { }
        }
    }

    #endregion

    #region Formation Initialization Functions

    private void InitializeInverted4Triangle()
    {
        //make sure we have four characters, otherwise inverted triangle 4 formation is impossible.
        if (UnitCount == 4)
        {
            //in order to do this we need to know what is the forward direction of the formation's anchor since one of them will
            //be placed in the back of the formation and the two others to either at the front.
            characters[0].transform.position = this.Position - 2.0f / 3.0f * formation_scale * this.transform.forward;
            characters[1].transform.position = this.Position + 1.0f / 3.0f * formation_scale * this.transform.right;
            characters[2].transform.position = this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right;
            characters[3].transform.position = this.Position + 1.0f / 3.0f * formation_scale * this.transform.forward;
        }
    }

    private void InitializeInvertedTriangle()
    {
        //make sure we have three characters, otherwise inverted triangle formation is impossible.
        if (UnitCount == 3)
        {
            //in order to do this we need to know what is the forward direction of the formation's anchor since one of them will
            //be placed in the back of the formation and the two others to either at the front.
            characters[0].transform.position = this.Position - 2.0f / 3.0f * formation_scale * this.transform.forward;
            characters[1].transform.position = this.Position + 1.0f / 3.0f * formation_scale * this.transform.right;
            characters[2].transform.position = this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right;
        }
    }

    private void InitializeTriangle()
    {
        //make sure we have three characters, otherwise triangle formation is impossible.
        if(UnitCount == 3)
        {
            //in order to do this we need to know what is the forward direction of the formation's anchor since one of them will
            //be placed at the head of the formation and the two others to either side.
            characters[0].transform.position = this.Position + 2.0f / 3.0f * formation_scale* this.transform.forward;
            characters[1].transform.position = this.Position + 1.0f / 3.0f * formation_scale * this.transform.right;
            characters[2].transform.position = this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right;
        }
    }

    private void InitializeSidePair()
    {
        if(UnitCount == 2)
        {
            characters[0].Position = this.Position + 1.0f / 3.0f * formation_scale* this.transform.right;
            characters[1].Position = this.Position + 1.0f / 3.0f * formation_scale * (-1.0f) * this.transform.right;
        }
    }

    #endregion

    #region Unity Mono Behaviour Overrides

    // Start is called before the first frame update
    void Start()
    {
        characters = new List<Character>();
        character = this.GetComponent<Character>();

        //first populate the list of characters.
        foreach(GameObject o in npcs)
        {
            characters.Add(o.GetComponent<Character>());
            o.transform.localPosition = Vector3.zero;
            characters[characters.Count - 1].Immobilized = true;
        }

        switch (formation_type)
        {
            case FORMATION_TYPE.INVERTED_4_TRIANGLE: InitializeInverted4Triangle(); break;
            case FORMATION_TYPE.INVERTED_TRIANGLE: InitializeInvertedTriangle(); break;
            case FORMATION_TYPE.TRIANGLE: InitializeTriangle(); break;
            case FORMATION_TYPE.SIDE_PAIR: InitializeSidePair(); break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UnitCount == 0)
            Destroy(this.gameObject);

       //we need to set the positions based on the formation.
       switch (formation_type)
       {
           case FORMATION_TYPE.INVERTED_4_TRIANGLE: SetInverted4Triangle(); break;
           case FORMATION_TYPE.INVERTED_TRIANGLE: SetInvertedTriangle(); break;
           case FORMATION_TYPE.TRIANGLE: SetTriangle(); break;
           case FORMATION_TYPE.SIDE_PAIR: SetSidePair(); break;
       }

       Debug.DrawRay(new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, this.transform.position.z), this.transform.forward, Color.red);
    }

    #endregion
}
