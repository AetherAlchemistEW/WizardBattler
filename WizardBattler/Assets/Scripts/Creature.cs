using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
    //VARIABLES
    public float attackDamage;
    public float defence;
    public float spellDamage;
    public float movementSpeed;

    public CombatType combatType;
    public ArmourType armour;

    public float[] eleMulti; //0-air,1-earth,2-water,3-fire

    //resistances
    public Element incDamageFrom;
    public Element lessDamageTo;
    
    //advantages
    public Element incDamageTo;
    public Element lessDamageFrom;

    private int health = 100;

    private List<GameObject> enemies;

    public Image healthBar;

    //Aesthetic tweaks
    //Get all visual addons (horns, armour, ect) and destroy all but one

    void Awake()
    {
        enemies = new List<GameObject>();

        if (combatType == CombatType.Defence)
        {

        }
        else if (combatType == CombatType.Melee)
        {

        }
        else if (combatType == CombatType.Ranged)
        {

        }
        else if (combatType == CombatType.Spell)
        {

        }

        StartCoroutine("attackPriorityCalc");
    }

    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            if(enemies.Count > 0)
            {
                Attack(enemies[0]);
            }
        }

        //We died
        if(health <= 0)
        {
            StopAllCoroutines();
            //remove us from all enemy threat lists
            string otherTeam = gameObject.tag == "Player1" ? "Player2" : "Player1";
            foreach (GameObject e in GameObject.FindGameObjectsWithTag(otherTeam))
            {
                if (e != null && e.GetComponent<Creature>().enemies.Contains(this.gameObject))
                {
                    e.GetComponent<Creature>().enemies.Remove(this.gameObject);
                }
            }
            //Destroy us
            Destroy(this.gameObject);
        }
    }

    //check for path through defences

    //find ideal defensive position

    //ranged attack

    //melee attack

    //spells

    //attack priority calculation
    IEnumerator attackPriorityCalc()
    {
        //Add any new enemies in the scene to our list
        string otherTeam = gameObject.tag == "Player1" ? "Player2" : "Player1";
        foreach (GameObject e in GameObject.FindGameObjectsWithTag(otherTeam))
        {
            if(!enemies.Contains(e) && e.GetComponent<Creature>())
            {
                enemies.Add(e);
            }
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine("attackPriorityCalc");
    }

    //defence priority calculation

    //Attack functionality
    void Attack(GameObject target)
    {
        float damage = attackDamage >= spellDamage ? attackDamage : spellDamage;
        target.GetComponent<Creature>().Hit(combatType, eleMulti, incDamageTo, lessDamageTo, damage);
    }

    //damage calculation from hit
    public void Hit(CombatType cType, float[] passedEleMulti, Element incDam, Element lessDam, float baseDam)
    {
        //Need: Attack type, armour type, ele multipliers, inc damage to, less damage to, attack or spell damage
        //if their attack type matches our armour type, halve damage bases
        baseDam *= AttackArmourMultiCalc(cType);
        //Double eleMultiplier for (their)inc damage to and (our)inc damage from and halve multiplier from (their)less damage to and (our)less damage from
        //% of each element(theirs)
        //% of each element(ours)
        float[] theirEleMulti = new float[4];
        float[] tempEleMulti = new float[4];
        for (int i = 0; i < tempEleMulti.Length; i++)
        {
            theirEleMulti[i] = passedEleMulti[i];
            theirEleMulti[i] /= 4;
            tempEleMulti[i] = eleMulti[i];
            tempEleMulti[i] /= 4;
        }
        //influence our resistance
        //increase damage
        if(incDam.name == "Air")
        {
            tempEleMulti[0] *= 2;
        }
        else if(incDam.name == "Earth")
        {
            tempEleMulti[1] *= 2;
        }
        else if (incDam.name == "Water")
        {
            tempEleMulti[2] *= 2;
        }
        else if (incDam.name == "Fire")
        {
            tempEleMulti[3] *= 2;
        }
        //less damage
        if (lessDam.name == "Air")
        {
            tempEleMulti[0] /= 2;
        }
        else if (lessDam.name == "Earth")
        {
            tempEleMulti[1] /= 2;
        }
        else if (lessDam.name == "Water")
        {
            tempEleMulti[2] /= 2;
        }
        else if (lessDam.name == "Fire")
        {
            tempEleMulti[3] /= 2;
        }
        //influence their damage
        //increase against our weakness
        if (incDamageFrom.name == "Air")
        {
            theirEleMulti[0] *= 2;
        }
        else if (incDamageFrom.name == "Earth")
        {
            theirEleMulti[1] *= 2;
        }
        else if (incDamageFrom.name == "Water")
        {
            theirEleMulti[2] *= 2;
        }
        else if (incDamageFrom.name == "Fire")
        {
            theirEleMulti[3] *= 2;
        }
        //reduce against our weakness
        if (lessDamageFrom.name == "Air")
        {
            theirEleMulti[0] /= 2;
        }
        else if (lessDamageFrom.name == "Earth")
        {
            theirEleMulti[1] /= 2;
        }
        else if (lessDamageFrom.name == "Water")
        {
            theirEleMulti[2] /= 2;
        }
        else if (lessDamageFrom.name == "Fire")
        {
            theirEleMulti[3] /= 2;
        }
        //Total up the damage they'll be dealing (average of their multiplier applied to base damage)
        //Total up our defence (average of our multiplier applied to base damage)
        float bonusDam = 0;
        float bonusResist = 0;
        for (int i = 0; i < tempEleMulti.Length; i++)
        {
            theirEleMulti[i] *= baseDam;
            tempEleMulti[i] *= defence;
            bonusDam += theirEleMulti[i];
            bonusResist += tempEleMulti[i];
        }
        bonusDam /= 4;
        bonusResist /= 4;

        int totalDamage = Mathf.Abs(Mathf.RoundToInt(bonusDam - bonusResist));
        Debug.Log(totalDamage);
        health -= totalDamage;
        healthBar.fillAmount = ((float)health / 100.0f);
    }

    //Attack type/armour type multiplier
    float AttackArmourMultiCalc(CombatType cType)
    {
        float multi = 1;
        if(armour == ArmourType.Melee && cType == CombatType.Melee)
        {
            multi -= 0.5f;
        }
        else if (armour == ArmourType.Ranged && cType == CombatType.Ranged)
        {
            multi -= 0.5f;
        }
        else if (armour == ArmourType.Magical && cType == CombatType.Spell)
        {
            multi -= 0.5f;
        }
        else if (armour == ArmourType.Mixed)
        {
            multi -= 0.25f;
        }
        return multi;
    }
}
