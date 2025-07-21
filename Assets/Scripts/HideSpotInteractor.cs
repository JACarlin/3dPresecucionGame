using UnityEngine;

public class HideSpotInteractor : MonoBehaviour
{
    [Header("Posiciones de Escondite/Salida")]
    public Transform hidingPosition; // Posición dentro del escondite.
    public Transform exitPosition;   // Posición de salida frente al escondite.

    [Header("Configuración de Interacción")]
    public float interactDistance = 3f; // Distancia máxima de interacción.

    private bool playerHidden = false;
    private GameObject player;
    private PlayerMovement playerMovement;

    // Collider del escondite
    private BoxCollider hideSpotCollider;

    void Awake()
    {
        // Busca el BoxCollider en el objeto actual o en sus hijos
        hideSpotCollider = GetComponentInChildren<BoxCollider>();

        if (hideSpotCollider == null)
        {
            Debug.LogError("¡No hay BoxCollider en el objeto o sus hijos!");
        }
        else
        {
            Debug.Log("Collider encontrado: " + hideSpotCollider.name);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerMovement = player.GetComponent<PlayerMovement>();
                }
            }

            if (player == null)
            {
                Debug.LogError("Jugador no encontrado.");
                return;
            }

            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance > interactDistance)
            {
                Debug.Log("Estás demasiado lejos para interactuar.");
                return;
            }

            if (!playerHidden)
            {
                Debug.Log(hideSpotCollider);
                EnterHideSpot();
            }
            else
                ExitHideSpot();
        }
    }

    void EnterHideSpot()
    {
        // Desactivar el collider del escondite para que no empuje al jugador.
        Debug.Log(hideSpotCollider);
        if (hideSpotCollider != null)
            hideSpotCollider.enabled = false;

        player.transform.position = hidingPosition.position;
        player.transform.rotation = exitPosition.rotation;

        if (playerMovement != null)
            playerMovement.enabled = false;

        FirstPersonMovement.Instance.ApplyCameraRestriction(exitPosition);

        playerHidden = true;
        Debug.Log("Jugador escondido.");
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.currentHideSpot = this;
        }
    }

    void ExitHideSpot(bool repositionPlayer = true)
    {
        if (repositionPlayer)
        {
            // Si se permite reposicionar, se mueve al jugador a la posición de salida del armario.
            player.transform.position = exitPosition.position;
            player.transform.rotation = exitPosition.rotation;
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        FirstPersonMovement.Instance.RemoveCameraRestriction();

        // Reactivar el collider del escondite.
        if (hideSpotCollider != null)
            hideSpotCollider.enabled = true;

        playerHidden = false;
        Debug.Log("Jugador salió del escondite.");
        PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.currentHideSpot = null;

    }
    public void ForceExitHideSpot()
    {
        if (playerHidden)
        {
            Debug.Log("player fue atrapado");
            ExitHideSpot(false);
        }
    }

}
