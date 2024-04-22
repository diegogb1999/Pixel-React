using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LayoutPlayer : MonoBehaviour
{
    public Button button; // El bot�n padre
    public Image childImage; // La imagen hijo

    private void Reset()
    {
        // Intentar auto-llenar los campos si no est�n asignados
        if (button == null)
            button = GetComponentInParent<Button>();
        if (childImage == null)
            childImage = GetComponent<Image>();
    }

    void Start()
    {
        if (button != null)
        {
            // A�adir listener para cuando el estado del bot�n cambie
            button.onClick.AddListener(ApplyColorChange);
        }
    }

    void ApplyColorChange()
    {
        if (childImage != null && button != null)
        {
            // Copiar el color actual del bot�n al hijo imagen
            ColorBlock colors = button.colors;
            childImage.color = button.image.color; // Propagar el color de la imagen del bot�n
        }
    }

    void OnDestroy()
    {
        if (button != null)
        {
            // Quitar listener cuando el script se destruya
            button.onClick.RemoveListener(ApplyColorChange);
        }
    }
}
