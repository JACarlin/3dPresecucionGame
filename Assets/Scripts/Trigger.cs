using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public NpcPursuit npcPursuit;  // Referencia al script del NPC
    public AudioSource ambientMusic;  // Música de ambiente
    public AudioSource actionMusic;   // Música cuando el jugador entra
    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador activó el switch del trigger.");
            if (npcPursuit != null)
            {
                npcPursuit.ActivateChaseSwitch(other);
            }
            // Se comenta la activación directa de la música, ya que se controlará en Update
            // AudioManager.instance.PlayActionMusic();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // No se realiza acción aquí, pues la música se controla de forma centralizada
    }

    // En cada frame se verifica el estado de todos los NPC y se ajusta la música en consecuencia
    private void Update()
    {
        // Obtiene todos los NPC en la escena
        NpcPursuit[] npcs = FindObjectsOfType<NpcPursuit>();
        bool algunPersiguiendo = false;
        foreach (NpcPursuit npc in npcs)
        {
            if (npc.IsChasing)
            {
                algunPersiguiendo = true;
                break;
            }
        }

        if (algunPersiguiendo)
        {
            // Si algún NPC está persiguiendo, se activa la música de acción
            if (!AudioManager.instance.actionMusic.isPlaying)
            {
                AudioManager.instance.PlayActionMusic();
            }
        }
        else
        {
            // Si ninguno está persiguiendo, se activa la música ambiental
            if (!AudioManager.instance.ambientMusic.isPlaying)
            {
                AudioManager.instance.PlayAmbientMusic();
            }
        }
    }

}
