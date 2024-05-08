using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonFix : MonoBehaviour, IPointerClickHandler
{
    private Button button;
    [SerializeField] private KeyCode activationKey;

    private void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    void Update ()
    {
        if (Input.GetKeyDown(activationKey))
        {
            StartCoroutine(SimulateButtonPress());
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    IEnumerator SimulateButtonPress()
    {
        // Guardar el color original para poder restablecerlo
        Color originalColor = button.colors.normalColor;
        Color pressedColor = button.colors.pressedColor;

        // Cambiar al color presionado
        SetButtonColor(pressedColor);

        // Esperar un breve periodo para simular que el botón está siendo presionado
        yield return new WaitForSeconds(0.065f);

        // Restablecer al color normal
        SetButtonColor(originalColor);
    }

    void SetButtonColor(Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        button.colors = colors;
    }
}

