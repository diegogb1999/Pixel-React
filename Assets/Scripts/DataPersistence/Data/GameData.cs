using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int levelsUnlocked;
    public string gameName;
    public int deathCount;

    public GameData()
    {
        this.levelsUnlocked = 1;
        this.gameName = "";
        this.deathCount = 0;
}
    public int LevelsUnlocked
    {
        get { return levelsUnlocked; }
    }
    public string GameName
    {
        get { return gameName; }
    }
    public int DeathCount
    {
        get { return deathCount; }
    }

    public void UnlockLevel()
    {
        levelsUnlocked++;
    }

    public void sumDeath()
    {
        deathCount++;
    }

    public void saveName(string name)
    {
        this.gameName = name;
    }
}
