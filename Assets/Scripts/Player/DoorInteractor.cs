using UnityEngine;

public class DoorInteractor : MonoBehaviour
{
    public float interactDistance;
    // Si no deseas filtrar por capas, asigna todos los layers:
    public LayerMask interactableLayers = ~0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Crear un rayo desde el centro de la pantalla.
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            // Dibuja el rayo para depuración
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red, 1f);

            if (Physics.Raycast(ray, out hit, interactDistance, interactableLayers))
            {
                // Busca el componente DoorController en el objeto o en sus padres.
                DoorController door = hit.collider.GetComponentInParent<DoorController>();
                if (door != null)
                {
                    door.Interact(gameObject);
                }
                else
                {
                    Debug.Log("No se encontró DoorController en el objeto o sus padres.");
                }
            }
            else
            {
                Debug.Log("Raycast sin colisión dentro del rango especificado.");
            }
        }
    }
}
