﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    private bool isActive;
    public GameManager gm;
    private Element[] combo;
    private int comboCounter;
    //air, earth, water, fire
    public Element[] baseElements;
    public GameObject UI;

    public Vector3 SumPoint;

    private int[] manaPool = { 5, 5, 5, 5 };
    public Text[] manaUI;

    public int health;

    void Awake()
    {
        for (int i = 0; i < manaPool.Length; i++)
        {
            manaUI[i].text = manaPool[i].ToString();
        }
    }

    // Update is called once per frame
	IEnumerator PlayerTurn()
    {
        for(int i = 0; i < manaPool.Length; i++)
        {
            if(manaPool[i] < 5)
            {
                manaPool[i]++;
                manaUI[i].text = manaPool[i].ToString();
            }
        }
        isActive = true;
        combo = new Element[4];
        comboCounter = 0;
        UI.SetActive(true);
        while (isActive)
        {
            if(combo[3] != null)
            { 
                isActive = false;
                UI.SetActive(false);
            }
            yield return null;
        }
        gm.NextPhase();
    }

    public void AddElement(int type)
    {
        //if we have mana of that type
        if (manaPool[type] > 0)
        {
            //reduce mana of that type, update UI
            manaPool[type]--;
            manaUI[type].text = manaPool[type].ToString();
            //add it to our combo and go to the next part of the sequence
            combo[comboCounter] = baseElements[type];
            comboCounter++;
        }
    }

    IEnumerator Summon()
    {
        string name = "";
        //Get creature base from first (well, last) element 
        //Spawn prefab and assign to creature
        GameObject creature = (GameObject)Instantiate(combo[3].baseCreature, SumPoint, Quaternion.identity);
        Creature cc = creature.GetComponent<Creature>();
        Color cColor = Color.black;

        //Strengths/Resistances
        cc.lessDamageFrom = combo[2].lessDamageFrom;
        cc.lessDamageTo = combo[2].lessDamageTo;
        cc.incDamageFrom = combo[2].incDamageFrom;
        cc.incDamageTo = combo[2].incDamageTo;

        //Associated armour type
        cc.armour = combo[1].armourType;

        //Associated combat type
        cc.combatType = combo[0].combatType;

        //add all cumulative values
        cc.eleMulti = new float[4];
        for (int i = 0; i < combo.Length; i++)
        {
            cc.attackDamage += combo[i].attackDamage;
            cc.defence += combo[i].defence;
            cc.spellDamage += combo[i].spell;
            cc.movementSpeed += combo[i].movementSpeed;
            cColor += combo[i].additiveColor;
            if (combo[i].name == "Air")//0
            {
                cc.eleMulti[i] += 1;

                if(i == 0)
                {
                    //combat
                    name += "Ranged ";
                }
                else if(i == 1)
                {
                    //armour
                    name += "Ethereal ";
                }
                else if (i == 2)
                {
                    //advantages/resistances
                    name += "Airy ";
                }
                else if (i == 3)
                {
                    //shell
                    name += "Angel";
                }
            }
            else if (combo[i].name == "Earth")//1
            {
                cc.eleMulti[i] += 1;

                if (i == 0)
                {
                    //combat
                    name += "Tanky ";
                }
                else if (i == 1)
                {
                    //armour
                    name += "Bulletproof ";
                }
                else if (i == 2)
                {
                    //advantages/resistances
                    name += "Earthen ";
                }
                else if (i == 3)
                {
                    //shell
                    name += "Golem";
                }
            }
            else if (combo[i].name == "Water")//2
            {
                cc.eleMulti[i] += 1;

                if (i == 0)
                {
                    //combat
                    name += "Caster ";
                }
                else if (i == 1)
                {
                    //armour
                    name += "Spellshielded ";
                }
                else if (i == 2)
                {
                    //advantages/resistances
                    name += "Liquid ";
                }
                else if (i == 3)
                {
                    //shell
                    name += "Mermidon";
                }
            }
            else if (combo[i].name == "Fire")//3
            {
                cc.eleMulti[i] += 1;

                if (i == 0)
                {
                    //combat
                    name += "Melee ";
                }
                else if (i == 1)
                {
                    //armour
                    name += "Resistant ";
                }
                else if (i == 2)
                {
                    //advantages/resistances
                    name += "Flaming ";
                }
                else if (i == 3)
                {
                    //shell
                    name += "Demon";
                }
            }  
        }
        //cColor = new Color(cColor.r / 4, cColor.g / 4, cColor.b / 4, 1);
        creature.GetComponent<Renderer>().material.color = cColor;
        creature.tag = gameObject.tag;
        cc.Init();
        creature.name = name;

        yield return null;
        gm.NextPhase();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SumPoint, 1.0f);
    }

    public void Hit(int damage)
    {
        health -= damage;
    }
}