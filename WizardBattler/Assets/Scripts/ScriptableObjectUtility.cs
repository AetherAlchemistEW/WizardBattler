using UnityEngine;
using UnityEditor;
using System.IO;

//THIS SCRIPT IS SOURCED FROM THE UNITY WIKI! Slightly tweaked, Most comments added later.
public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        //Create an instance of our passed scriptable object
        T asset = ScriptableObject.CreateInstance<T>();

        //Path and asset name
        string path = "Assets";
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(T).ToString() + ".asset");

        //Make the instance an asset, update the asset database, force focus
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
