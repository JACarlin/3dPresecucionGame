using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    public Transform defaultCheckpoint; // Checkpoint por defecto

    private Transform lastCheckpoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // Establecer el checkpoint inicial
        lastCheckpoint = defaultCheckpoint;
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        lastCheckpoint = checkpoint;
        Debug.Log($"Checkpoint actualizado: {checkpoint.position}");
    }

    public Transform GetLastCheckpoint()
    {
        return lastCheckpoint != null ? lastCheckpoint : defaultCheckpoint;
    }
}
