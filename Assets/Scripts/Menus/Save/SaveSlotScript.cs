using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotScript : MonoBehaviour
{
    [Header("Profile")]

    [SerializeField] private string profileId = "";

    [Header("Content")]

    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;

    [SerializeField] private TextMeshProUGUI levelsUnlocked;
    [SerializeField] private TextMeshProUGUI gameName;
    [SerializeField] private TextMeshProUGUI deathCount;

    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        if (data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            levelsUnlocked.text = "Levels: " + data.levelsUnlocked.ToString();
            gameName.text = data.gameName.ToString();
            deathCount.text = "Deaths: " + data.deathCount.ToString();
        }
    }

    public string GetProfileId()
    {
        return this.profileId;
    }

    public void setInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }

    public string GetGameName()
    {
        return this.gameName.text;
    }
}
