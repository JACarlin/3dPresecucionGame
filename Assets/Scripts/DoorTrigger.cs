using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DoorTrigger : MonoBehaviour
{
    public DoorController doorController; // Asigna la puerta en el Inspector.
    public GameObject messageUI;          // Objeto UI (por ejemplo, un TextMeshProUGUI) para mostrar mensajes.

    private bool playerInTrigger = false;
    private int npcCount = 0;             // Contador de NPCs en el trigger.
    private float doorCloseDelay = 0.2f;  // Retraso antes de cerrar la puerta cuando no hay NPCs.

    void Start()
    {
        if (messageUI != null)
            messageUI.SetActive(false); // Oculta el mensaje al inicio.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            ShowMessage();
        }
        else if (other.CompareTag("NPC"))
        {
            doorController.pendingClose = true;
            npcCount++;
            doorController.OpenDoor(); // Abre la puerta automáticamente.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (messageUI != null)
                messageUI.SetActive(false); // Oculta el mensaje al salir.
        }
        else if (other.CompareTag("NPC"))
        {
            npcCount--;
            if (npcCount <= 0)
            {
                // Inicia una corutina para cerrar la puerta con un pequeño retraso.
                StartCoroutine(CloseDoorAfterDelay());
            }
        }
    }

    IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(doorCloseDelay);
        // Solo cierra la puerta si sigue sin haber NPCs.
        if (npcCount <= 0)
            doorController.CloseDoor();
    }

    void ShowMessage()
    {
        if (messageUI != null)
        {
            TextMeshProUGUI textComponent = messageUI.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                string messageKey;
                object[] formatArgs = Array.Empty<object>(); // Inicializa como array vacío

                if (doorController.IsOpen)
                {
                    messageKey = "door_close_prompt";
                }
                else
                {
                    if (doorController.requiredKey != null && !doorController.HasKey())
                    {
                        messageKey = "door_required_key";
                        formatArgs = new object[] { doorController.requiredKey.LocalizedName};
                    }
                    else
                    {
                        messageKey = "door_open_prompt";
                    }
                }

                textComponent.text = LanguageManager.Instance.GetText(messageKey, formatArgs);
                messageUI.SetActive(true);
            }
        }
    }


    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // Asume que el objeto con la etiqueta "Player" es el que interactúa.
            doorController.Interact(GameObject.FindGameObjectWithTag("Player"));
        }
    }
}
