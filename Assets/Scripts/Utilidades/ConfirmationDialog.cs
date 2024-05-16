using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ConfirmationDialog : MonoBehaviour
{
    public TMP_Text messageText;
    public Button confirmButton;
    public Button cancelButton;
    public GameObject blockPanel; // El objeto Image que cubre toda la pantalla

    public void Show(string message, UnityAction onConfirm, UnityAction onCancel)
    {
        messageText.text = message;
        blockPanel.SetActive(true); // Asegúrate de activar el panel de bloqueo aquí

        Debug.Log("Showing confirmation dialogaa."); // Añadir mensaje de depuración

        // Verificar si los elementos están correctamente asignados
        if (messageText == null) Debug.LogError("messageText is not assigned.");
        if (confirmButton == null) Debug.LogError("confirmButton is not assigned.");
        if (cancelButton == null) Debug.LogError("cancelButton is not assigned.");
        if (blockPanel == null) Debug.LogError("blockPanel is not assigned.");



        // Remover todos los listeners anteriores
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        // Agregar los nuevos listeners
        confirmButton.onClick.AddListener(() =>
        {
            Debug.Log("Confirm button clicked.");
            onConfirm?.Invoke();
            Close();
        });

        cancelButton.onClick.AddListener(() =>
        {
            Debug.Log("Cancel button clicked.");
            onCancel?.Invoke();
            Close();
        });
    }

    public void Close()
    {
        blockPanel.SetActive(false); // Desactivar la máscara de bloqueo
    }
}
