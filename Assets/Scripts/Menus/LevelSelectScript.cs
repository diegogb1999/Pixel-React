using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class LevelSelectScript : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject levelButtons;

    private Button[] buttons;

    int levelsUnlocked;

    private void Awake()
    {
        ButtonsToArray();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        for (int i = 0; i < levelsUnlocked; i++)
        {
            buttons[i].interactable = true;
        }
    }
    private void Start()
    {
        UpdateButtons();  // Asegúrate de llamar a UpdateButtons al inicio para configurar correctamente los botones.
    }

    public void OnClickLevel(int level)
    {
        GameManager.instance.LoadScene(level);

        //Hacer que todos los botones sean no interactables
        //UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>().enabled = false;
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

    public void LoadData(GameData data)
    {
        levelsUnlocked = data.levelsUnlocked;
        UpdateButtons();
        Debug.Log($"Cargando datos: Niveles Desbloqueados = {levelsUnlocked}");
    }

    private void UpdateButtons()
    {
        if (buttons == null || buttons.Length == 0)
        {
            //Debug.LogError("Botones no inicializados o array vacío");
            return;  // Salir si los botones no están asignados.
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = i < levelsUnlocked;
        }

        
    }

    public void SaveData(GameData data)
    {
        
        data.levelsUnlocked = levelsUnlocked;
        Debug.Log($"Guardando datos: Niveles Desbloqueados = {data.levelsUnlocked}");
    }
}
