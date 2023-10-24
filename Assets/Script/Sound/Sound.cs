using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip audioClip;
}

public static class SoundConstants
{
    public const string BACKGROUND = "Background";
    public const string WIN = "Win";
    public const string LOSE = "Lose";
    public const string COLLECT_TILE = "CollectTile";
    public const string SELECT_TILE = "SelectTile";
}
