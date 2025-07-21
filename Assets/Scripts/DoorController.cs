using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Configuración de llave")]
    // Si requiredKey es null, la puerta se abrirá sin llave.
    public ItemData requiredKey;

    [Header("Animación de Puerta")]
    // Rotación en estado cerrado (se asigna automáticamente en Start si no se define)
    public Vector3 closedRotation;
    // Rotación en estado abierto (se asigna automáticamente en Start si no se define)
    public Vector3 openRotation;
    // Duración de la animación en segundos
    public float animationDuration = 1f;

    [Header("Opcionales: Collider y NavMesh")]
    public Collider doorCollider;           // Collider de la puerta.
    public NavMeshObstacle navMeshObstacle;   // Para que la puerta no bloquee el NavMesh.

    public bool isOpen = false;
    private bool keyUsed = false;           // Indica si ya se usó la llave.
    private bool isAnimating = false;
    private Transform doorPivot;
    public bool pendingClose = false;


    void Start()
    {
        // Asignar componentes si no se han configurado en el Inspector.
        if (doorCollider == null)
            doorCollider = GetComponent<Collider>();

        if (navMeshObstacle == null)
            navMeshObstacle = GetComponent<NavMeshObstacle>();

        if (doorPivot == null)
            doorPivot = transform.parent != null ? transform.parent : transform;

        // Si no se asignó una rotación cerrada, usamos la rotación actual de la puerta.


        // Si no se asignó una rotación abierta, asumimos que la puerta abre 90° sobre Y.
        if (openRotation == Vector3.zero)
            openRotation = closedRotation + new Vector3(0, 90f, 0);
    }

    // Se llama al interactuar la puerta (desde el jugador o el NPC)
    public void Interact(GameObject interactor)
    {
        if (!isOpen)
        {

            // Si es el jugador
            if (interactor.CompareTag("MainCamera"))
            {

                // Si se requiere llave y no se ha usado aún.
                if (requiredKey != null && !keyUsed)
                {

                    InventarioController inventory = GameObject.FindObjectOfType<InventarioController>();

                    Debug.Log(inventory);
                    if (inventory != null)
                    {
                        ItemData selectedItem = inventory.GetSelectedItemData();
                        if (selectedItem != null && selectedItem == requiredKey)
                        {
                            // Remueve la llave y marca que ya se usó.
                            inventory.RemoveSelectedItem();
                            keyUsed = true;
                            OpenDoor();
                        }
                        else
                        {
                            Debug.Log("Necesitas la llave correcta para abrir esta puerta.");
                        }
                    }
                }
                else
                {
                    Debug.Log("adsada");
                    OpenDoor();
                }
            }

            // Si es un NPC (asegúrate de que tenga la etiqueta "NPC")
            else if (interactor.CompareTag("NPC"))
            {
                OpenDoor();
            }
        }
        else
        {
            // Si la puerta está abierta, se cierra.
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        if (isAnimating) return;
        isOpen = true;

        StartCoroutine(AnimateDoor(Quaternion.Euler(openRotation)));
        Debug.Log("Puerta abriéndose.");
    }

    public void CloseDoor()
    {
        if (isAnimating)
        {
              // Se guarda la orden de cerrar para cuando termine la animación.
            return;
        }
        isOpen = false;
        StartCoroutine(AnimateDoor(Quaternion.Euler(closedRotation)));
        Debug.Log("Puerta cerrándose.");
    }


    IEnumerator AnimateDoor(Quaternion targetRotation)
    {
        isAnimating = true;
        Quaternion startRotation = doorPivot.rotation;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            doorPivot.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        doorPivot.rotation = targetRotation;
        isAnimating = false;

        // Si se solicitó cerrar mientras animaba, ejecuta la orden.
        if (pendingClose && isOpen)
        {
            pendingClose = false;
            CloseDoor();
        }
    }

    public bool HasKey()
    {
        // Si la puerta no requiere llave, retorna true.
        if (requiredKey == null) return true;
        // Si la llave ya fue usada, la puerta ya está desbloqueada.
        if (keyUsed) return true;

        InventarioController inventory = GameObject.FindObjectOfType<InventarioController>();
        if (inventory != null)
        {
            ItemData selectedItem = inventory.GetSelectedItemData();
            return selectedItem != null && selectedItem == requiredKey;
        }
        return false;
    }
    public bool IsOpen
    {
        get { return isOpen; }
    }



}
