using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectScript : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;

    private void Awake()
    {
        ButtonsToArray();
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        for (int i = 0; i < unlockedLevel; i++)
        {
            buttons[i].interactable = true;
        }
    }

    public void OnClickLevel(int level)
    {
        GameManager.instance.LoadScene(level);
    }

    void ButtonsToArray()
    {
        int childCount = levelButtons.transform.childCount;
        buttons = new Button[childCount];
        for (int i = 0;i < childCount;i++)
        {
            buttons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }
}
