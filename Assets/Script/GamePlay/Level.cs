using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class Level : IEntity, IActivated
{
    public bool IsActivated => ActivatedLevelId == Id;
    [field: SerializeField] public int Id {get; private set;}
    public string Name {get; private set;}
    public LevelSettings LevelSettings;
    public int MatchingCount;
    public int LevelIndex;
    public int PlayTime;
    public int NumberOfStar;
    public TileItems[] TileType;

    public static readonly string ActivatedLevelKey = "ActivatedLevel";
    public static int ActivatedLevelId => PlayerPrefs.GetInt(ActivatedLevelKey);

    public Level(LevelSettings levelSettings)
    {
        Name = levelSettings.Name;
        MatchingCount = levelSettings.MatchingCount;
        LevelIndex = levelSettings.Level;
        PlayTime = levelSettings.PlayTime;
        NumberOfStar = levelSettings.NumberOfStar;
        TileType = levelSettings.TileTypes;
    }

    public void Activate()
    {
        PlayerPrefs.SetInt(ActivatedLevelKey, Id);
    }
    public void DeActivate()
    {
        PlayerPrefs.DeleteKey(ActivatedLevelKey);
    }
}
