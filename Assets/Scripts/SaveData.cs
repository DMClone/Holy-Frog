using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    // public GameObject /
    public int levelsUnlocked;
    public TimeData[] timeDatas;
}

public struct TimeData
{
    public float time;
    public Medal medal;
    
}

public enum Medal
{
    Bronze,
    Silver,
    Gold
}
