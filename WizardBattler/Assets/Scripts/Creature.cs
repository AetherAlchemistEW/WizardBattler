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

    [SerializeField]
    private int health = 100;

    public List<GameObject> enemies;
    public List<GameObject> allies;

    public Image healthBar;

    [SerializeField]
    private PlayerController enemyPlayer;
    [SerializeField]
    private PlayerController ourPlayer;

    [SerializeField]
    private List<string> actions;

    private Vector3 movementPosition;

    private GameObject attackTarget;

    //Aesthetic tweaks
    //Get all visual addons (horns, armour, ect) and destroy all but one

    public void Init()
    {
        //intialise lists
        enemies = new List<GameObject>();
        actions = new List<string>();
        allies = new List<GameObject>();

        //set our enemy and owned player
        foreach (PlayerController p in GameObject.FindObjectsOfType<PlayerController>())
        {
            string enemyTeam = gameObject.tag == "Player1" ? "Player2" : "Player1";
            if(p.gameObject.CompareTag(enemyTeam))
            {
                enemyPlayer = p;
            }
            else
            {
                ourPlayer = p;
            }
        }

        //Based on our combat type, add our main behaviour to the actions list
        if (combatType == CombatType.Defence)
        {
            actions.Add("DefensivePositions");
        }
        else if (combatType == CombatType.Melee)
        {
            actions.Add("AttackPriorityCalc");
        }
        else if (combatType == CombatType.Ranged)
        {
            actions.Add("AttackPriorityCalc");
        }
        else if (combatType == CombatType.Spell)
        {
            actions.Add("AttackPriorityCalc");
        }
        //actions.Add("DefensivePositions");
        //Add populating our lists (enemies and allies) to the actions list
        actions.Add("PopulateLists");
        //Begin our actions
        BeginActions();
    }

    public void RemoveElement(GameObject target)
    {
        StopAllCoroutines();
        if (enemies.Contains(this.gameObject))
        {
            enemies.Remove(this.gameObject);
        }
        else if (allies.Contains(this.gameObject))
        {
            allies.Remove(this.gameObject);
        }
        BeginActions();
    }

    void Update()
    {
        //We died
        if (health <= 0)
        {
            //stop all actions
            StopAllCoroutines();
            //remove us from all enemy and ally lists, consolidate lists for the purpose of streamlining this
            //enemies.AddRange(allies);
            List <GameObject> all = new List<GameObject>();
            all.AddRange(enemies);
            all.AddRange(allies);
            foreach (GameObject e in all)
            {
                if (e != null && !e.GetComponent<PlayerController>()) //quick null and player check
                {
                    e.GetComponent<Creature>().RemoveElement(this.gameObject);
                }
            }
            //Destroy us
            Destroy(this.gameObject);
        }
    }

    void BeginActions()
    {
        //Stop everything
        StopAllCoroutines();
        //Start all actions, pseudo-delegate coroutine system
        foreach (string action in actions)
        {
            StartCoroutine(action);
        }
    }

    //check for path to attack
    IEnumerator FindAttackPosition()
    {
        int distance = combatType == CombatType.Melee ? 4 : 10;
        if(Vector3.Distance(transform.position, attackTarget.transform.position) > distance)
        {
            movementPosition = attackTarget.transform.position + ((transform.position - attackTarget.transform.position).normalized * (distance-1));
            //Debug.Log("Reposition");
        }
        else
        {
            //Debug.Log("Close Enough");
            //check LOS and adjust
            RaycastHit hit;
            Debug.DrawRay(transform.position, (attackTarget.transform.position - transform.position).normalized * 10);
            if(Physics.Raycast(transform.position, (attackTarget.transform.position - transform.position).normalized, out hit))
            {
                if(hit.transform.gameObject == attackTarget)
                {
                    //Debug.Log("See Target");
                    yield return new WaitForSeconds(0.2f);
                    StartCoroutine("Attack");
                }
                else
                {
                    //Debug.Log("Not Target");
                    int newZ = transform.position.z > 0 ? -10 : 10;
                    int newX = gameObject.tag == "Player1" ? 10 : -10;
                    movementPosition += new Vector3(newX, 0 , newZ);
                }
            }
        }
        yield return null;
        StartCoroutine("MoveToPoint");
    }

    //ranged attack

    //melee attack

    //spells

    //populate enemies and allies lists
    IEnumerator PopulateLists()
    {
        //Check all the creatures in the scene, if they're on our team put them in the enemies list, otherwise put them in the allies list
        string otherTeam = gameObject.tag == "Player1" ? "Player2" : "Player1";
        foreach (Creature e in GameObject.FindObjectsOfType<Creature>())
        {
            if (!enemies.Contains(e.gameObject) && e.gameObject.CompareTag(otherTeam))
            {
                enemies.Add(e.gameObject);
            }
            else if (!allies.Contains(e.gameObject) && e.gameObject.CompareTag(gameObject.tag))
            {
                allies.Add(e.gameObject);
            }
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine("PopulateLists");
    }

    //attack priority calculation
    IEnumerator AttackPriorityCalc()
    {
        //list of enemies and other player, find attack targe based on priority calculation.
        List<GameObject> potentialTargets = new List<GameObject>();
        potentialTargets.AddRange(enemies);
        potentialTargets.Add(enemyPlayer.gameObject);
        if (potentialTargets.Count > 1 && potentialTargets[0] != null)
        {
            attackTarget = potentialTargets[0];
            float targetPriority = CalcPriority(attackTarget.transform);
            for (int i = 0; i < potentialTargets.Count; i++)
            {
                if(potentialTargets[i] == null)
                {
                    continue;
                }
                else if (targetPriority < CalcPriority(potentialTargets[i].transform))
                {
                    attackTarget = potentialTargets[i];
                }
            }
        }
        else
        {
            attackTarget = enemyPlayer.gameObject;
        }

        //after the loop the current attack player should be 'attacked'
        StartCoroutine("FindAttackPosition");
        yield return new WaitForSeconds(0.2f);
        StartCoroutine("AttackPriorityCalc");
    }

    float CalcPriority(Transform target)
    {
        //if the enemy is closer or lower health or higher damage ('threat/convenience') 
        //player gets a flat (high) value instead of damage
        float priority = 0;
        priority += (10 - Vector3.Distance(attackTarget.transform.position, target.transform.position));
        if (target.GetComponent<PlayerController>())
        {
            priority += 1 - target.GetComponent<PlayerController>().health / 100;
            //priority += 5;
            //TODO: distance to our player
        }
        else if (target.GetComponent<Creature>())
        {
            Creature c = target.GetComponent<Creature>();
            priority += 1 - (c.health / 100);
            priority += c.attackDamage > c.spellDamage ? c.attackDamage : c.spellDamage;
            priority -= c.defence/2;
        }
        return priority;
    }

    //move to position
    IEnumerator MoveToPoint()
    {
        movementPosition = new Vector3(movementPosition.x, transform.position.y, movementPosition.z);
        while(Vector3.Distance(transform.position, movementPosition) > 0.2f)
        {
            Vector3 oldPos = transform.position;
            transform.position = Vector3.Lerp(transform.position, movementPosition, movementSpeed * 0.2f * Time.smoothDeltaTime);
            transform.LookAt(transform.position - oldPos);
            yield return null;
        }
        yield return null;
    }

    //find ideal defensive position
    IEnumerator DefensivePositions()
    {
        if (enemies.Count > 0)
        {
            Vector3 enemyAvg = new Vector3();
            int x = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                if(enemies[i] == null)
                {
                    continue;
                }
                else
                {
                    enemyAvg += (ourPlayer.transform.position - enemies[i].transform.position);
                    x++;
                }
            }
            enemyAvg /= x;
            enemyAvg.Normalize();

            movementPosition = ourPlayer.transform.position + (-enemyAvg * 2);
            StartCoroutine("MoveToPoint");
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine("DefensivePositions");
    }

    //Attack functionality
    IEnumerator Attack()
    {
        if (attackTarget != null)
        {
            float damage = attackDamage >= spellDamage ? attackDamage : spellDamage;
            if (attackTarget.GetComponent<Creature>())
            {
                attackTarget.GetComponent<Creature>().Hit(combatType, eleMulti, incDamageTo, lessDamageTo, damage, gameObject);
            }
            else
            {
                attackTarget.GetComponent<PlayerController>().Hit(Mathf.RoundToInt(damage));
            }
        }
        yield return new WaitForSeconds(0.2f);
    }

    //damage calculation from hit
    public void Hit(CombatType cType, float[] passedEleMulti, Element incDam, Element lessDam, float baseDam, GameObject attacker)
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
        //Debug.Log(totalDamage);
        health -= totalDamage;
        healthBar.fillAmount = ((float)health / 100.0f);

        if(combatType == CombatType.Defence && Vector3.Distance(transform.position, attacker.transform.position) < 2)
        {
            attackTarget = attacker;
            StartCoroutine("Attack");
        }
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

    void OnDrawGizmos()
    {
        Gizmos.color = gameObject.GetComponent<Renderer>().material.color;
        Gizmos.DrawWireSphere(movementPosition, 1.0f);
    }
}
