using UnityEngine;
using System.Collections;
using UnityEditor;

public enum CombatType
{
    Melee, Ranged, Spell, Defence
}
public enum ArmourType
{
    Melee, Ranged, Magical, Mixed
}

//Scriptable object for elements
public class Element : ScriptableObject
{
    //Associated stat bonuses, cumulative
    public float attackDamage;
    public float movementSpeed;
    public float defence;
    public float spell;
    public Color additiveColor;

    //Determined by pick order
    //FIRST
    //Associated combat type
    public CombatType combatType;

    //SECOND
    //Associated armour type
    public ArmourType armourType;

    //THIRD
    //Strengths/Resistances
    public Element incDamageFrom;
    public Element lessDamageFrom;
    public Element incDamageTo;
    public Element lessDamageTo;

    //FOURTH
    //Prefab (body)
    public GameObject baseCreature;

    //Method for creating an instance of this object in the asset directory
    [MenuItem("Assets/Create/Element")]
    public static void Create()
    {
        ScriptableObjectUtility.CreateAsset<Element>();
    }
}