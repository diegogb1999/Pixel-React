using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int levelsUnlocked;

    public GameData()
    {
        this.levelsUnlocked = 1;
    }
    public int LevelsUnlocked
    {
        get { return levelsUnlocked; }
    }

    public void UnlockLevel()
    {
        levelsUnlocked++;
    }
}
