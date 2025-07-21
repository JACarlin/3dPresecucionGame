using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public HideSpotInteractor currentHideSpot;

    // Start y Update según tus necesidades
    void Start() { }
    void Update() { }

    public void RespawnAtLastCheckpoint()
    {
        Transform checkpoint = CheckpointManager.Instance.GetLastCheckpoint();
        if (checkpoint != null)
        {
            transform.position = checkpoint.position;
            Debug.Log("Jugador respawneado en el último checkpoint.");
        }
        else
        {
            Debug.LogWarning("No hay un checkpoint guardado.");
        }

        // Reactivar el movimiento (suponiendo que tienes un componente PlayerMovement)
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("PlayerMovement reactivado.");
        }

        FirstPersonMovement.Instance.RemoveCameraRestriction();


        // Si el jugador estaba escondido en algún hide spot, forzar la salida
        if (currentHideSpot != null)
        {
            currentHideSpot.ForceExitHideSpot();
            currentHideSpot = null;
        }
    }

}
