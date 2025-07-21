using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public Transform[] waypoints; // Arreglo de waypoints
    public float speed = 3f; // Velocidad de movimiento
    private int currentWaypointIndex = 0; // Índice del waypoint actual

    void Update()
    {
        if (waypoints.Length == 0) return;

        // Mueve al NPC hacia el waypoint actual
        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Si llega al waypoint, pasa al siguiente
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
