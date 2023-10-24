using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : GameSingleton<LevelManager>
{
    [HideInInspector] public Level currentLevel;
    public LevelSettings[] levelSettings;

    protected override void Awake() 
    {
        base.Awake();

        if (!PlayerPrefs.HasKey(Level.ActivatedLevelKey))
        {
            currentLevel = new Level(levelSettings[0]);
            currentLevel.Activate();
        }
        else
        {
            currentLevel = LoadCurrentLevel();
        }
    }

    public Level LoadCurrentLevel()
    {
        var key = PlayerPrefs.GetInt(Level.ActivatedLevelKey);

        for (int i = 0; i < levelSettings.Length; i++)
        {
            if (levelSettings[i].Id == key)
                return new Level(levelSettings[i]);
        }

        return new Level(levelSettings[0]);
    }

    public void NextLevel()
    {
        Debug.Log("Level Index: " + currentLevel.LevelIndex + " Levelsettings: " + levelSettings.Length);
        if (currentLevel.LevelIndex == levelSettings.Length)
        {
            currentLevel = new Level(levelSettings[0]);
            currentLevel.Activate();
            return;
        }

        for (int i = 0; i < levelSettings.Length; i++)
        {
            if (levelSettings[i].Level == currentLevel.LevelIndex + 1)
            {
                currentLevel = new Level(levelSettings[i]);
                break;
            }
        }

        currentLevel.Activate();
    }
}
