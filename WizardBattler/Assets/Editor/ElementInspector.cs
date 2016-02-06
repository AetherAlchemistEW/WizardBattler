using UnityEngine;
using System.Collections;
using UnityEditor;

//Custom inspector for the element scriptable objects
[CustomEditor(typeof(Element))]
public class ElementInspector : Editor
{
    //Bools for handling collapsable sections
    bool cumulativeOpen = true;
    bool firstOpen = true;
    bool secondOpen = true;
    bool thirdOpen = true;
    bool fourthOpen = true;

    public override void OnInspectorGUI()
    {
        Element ele = (Element)target;

        cumulativeOpen = EditorGUILayout.Foldout(cumulativeOpen, "Cumulative Stats");
        if (cumulativeOpen)
        {
            //Cumulative values
            EditorGUILayout.HelpBox("Stats contributed to monster every inclusion", MessageType.Info);
            ele.attackDamage = EditorGUILayout.FloatField("Attack Damage", ele.attackDamage);
            ele.defence = EditorGUILayout.FloatField("Defence", ele.defence);
            ele.spell = EditorGUILayout.FloatField("Spell Damage", ele.spell);
            ele.movementSpeed = EditorGUILayout.FloatField("Movement Speed", ele.movementSpeed);
            ele.additiveColor = EditorGUILayout.ColorField("Element Colour", ele.additiveColor);
        }
        //First in sequence values
        firstOpen = EditorGUILayout.Foldout(firstOpen, "1: Type of attack");
        if (firstOpen)
        {
            //Cumulative values
            EditorGUILayout.HelpBox("Handles the type of attack", MessageType.Info);
            ele.combatType = (CombatType)EditorGUILayout.EnumPopup("Type of attack", ele.combatType);
        }
        //Second in sequence values
        secondOpen = EditorGUILayout.Foldout(secondOpen, "2: Type of armour");
        if (secondOpen)
        {
            //Cumulative values
            EditorGUILayout.HelpBox("Handles the type of armour", MessageType.Info);
            ele.armourType = (ArmourType)EditorGUILayout.EnumPopup("Type of armour", ele.armourType);
        }
        //Third in sequence values
        thirdOpen = EditorGUILayout.Foldout(thirdOpen, "3: Advantages and Resistances");
        if (thirdOpen)
        {
            //Resistances and advantages
            EditorGUILayout.HelpBox("Handles all advantages and resistances", MessageType.Info);
            ele.incDamageTo = (Element)EditorGUILayout.ObjectField("Increased damage to: ", ele.incDamageTo, typeof(Element), false);
            ele.incDamageFrom = (Element)EditorGUILayout.ObjectField("Increased damage from: ", ele.incDamageFrom, typeof(Element), false);
            ele.lessDamageTo = (Element)EditorGUILayout.ObjectField("Reduced damage to: ", ele.lessDamageTo, typeof(Element), false);
            ele.lessDamageFrom = (Element)EditorGUILayout.ObjectField("Reduced damage from: ", ele.lessDamageFrom, typeof(Element), false);
        }
        //Fourth in sequence values
        fourthOpen = EditorGUILayout.Foldout(fourthOpen, "4: Base Model");
        if (fourthOpen)
        {
            //Model
            EditorGUILayout.HelpBox("Handles the base model", MessageType.Info);
            ele.baseCreature = (GameObject)EditorGUILayout.ObjectField("Base Creature", ele.baseCreature, typeof(GameObject), false);
        }
        //base.OnInspectorGUI();
    }
}
