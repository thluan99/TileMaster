using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Settings/Level", fileName = "Level")]
public class LevelSettings : ScriptableObject
{
#if UNITY_EDITOR
    public int id;
#endif
    public int Id => (nameof(Level) + Name).GetHashCode();
    public string Name;
    public int MatchingCount = 3;
    public int Level;
    public int PlayTime;
    public int NumberOfStar;
    public TileItems[] TileTypes;
}

[Serializable]
public class TileItems : IEntity
{
    public int Id => (nameof(TileItems) + Name).GetHashCode();
    public string Name { get; set; }
    public Sprite TileSprite;
    public int NumberStar;
}

#if UNITY_EDITOR

[CustomEditor(typeof(LevelSettings))]
public class LevelSetttingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelSettings levelSettings = (LevelSettings)target;

        if (GUILayout.Button("Load Id"))
        {
            levelSettings.id = levelSettings.Id;
        }

        if (levelSettings.TileTypes.Length == 0) return;

        int countStar = 0;
        for (int i = 0; i < levelSettings.TileTypes.Length; i++)
        {
            countStar += levelSettings.TileTypes[i].NumberStar;
        }

        if (countStar != levelSettings.NumberOfStar)
        {
            EditorGUILayout.HelpBox("Number of star not equal total of star!" + countStar +
                                    "\nTotal star: " + countStar +
                                    "\nNumber of star: " + levelSettings.NumberOfStar, 
                                    MessageType.Error);
        }
    }
}
#endif