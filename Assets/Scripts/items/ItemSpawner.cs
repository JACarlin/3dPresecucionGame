using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Zonas de Spawn")]
    // Asigna en el Inspector las zonas de spawn que quieres reutilizar para todos los ítems.
    public Transform[] spawnZones;

    // Registro de zonas ya utilizadas (compartido entre todas las instancias de ItemSpawn)
    private static HashSet<Transform> usedZones = new HashSet<Transform>();
    // Almacena la zona elegida para este ítem, para liberarla al destruirse.
    private Transform chosenZone;

    void Start()
    {
        // Genera una lista de zonas libres (no ocupadas por otros ítems)
        List<Transform> freeZones = new List<Transform>();
        foreach (Transform zone in spawnZones)
        {
            if (!usedZones.Contains(zone))
            {
                freeZones.Add(zone);
            }
        }

        if (freeZones.Count > 0)
        {
            // Si hay zonas libres, se elige una al azar y se marca como ocupada.
            int randomIndex = Random.Range(0, freeZones.Count);
            chosenZone = freeZones[randomIndex];
            usedZones.Add(chosenZone);
        }
        else
        {
            // Si todas las zonas están ocupadas, se elige una zona al azar.
            // Aquí puedes modificar la lógica: por ejemplo, no spawnear el ítem o reiniciar el registro.
            chosenZone = spawnZones[Random.Range(0, spawnZones.Length)];
        }

        // Ubica el ítem en la posición y rotación de la zona elegida.
        transform.position = chosenZone.position;
        transform.rotation = chosenZone.rotation;
    }

    void OnDestroy()
    {
        // Al destruirse el ítem, libera la zona para que pueda usarse en el futuro.
        if (chosenZone != null && usedZones.Contains(chosenZone))
        {
            usedZones.Remove(chosenZone);
        }
    }
}
