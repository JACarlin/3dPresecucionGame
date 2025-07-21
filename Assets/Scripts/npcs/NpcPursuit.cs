using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NPCState { Patrolling, Chasing, Investigating }

public class NpcPursuit : MonoBehaviour
{
    public Transform[] waypoints;      // Waypoints para patrullar
    public Transform player;           // Referencia al Player (objeto raíz)
    public float patrolSpeed = 3f;     // Velocidad en patrulla
    public float chaseSpeed = 5f;      // Velocidad al perseguir
    public float transportOffset = 2f; // Distancia que se coloca el NPC frente al jugador

    // Variables de audio
    public AudioSource npcAudioSource;
    public AudioClip patrolClip;
    public AudioClip chaseClip;
    public AudioClip captureClip;

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    // Estado actual del NPC y variables para la detección
    private NPCState currentState = NPCState.Patrolling;
    private bool chaseSwitchActivated = false;   // Se activa al tocar el trigger
    private Vector3 lastKnownPlayerPosition;

    // Flag para controlar el audio de captura y evitar empalmes
    private bool isCapturing = false;
    private Animator animator;
    private bool isNoAnimationPlaying = false;

    public bool IsChasing
    {
        get { return currentState == NPCState.Chasing; }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No se encontró NavMeshAgent en el NPC.");
            return;
        }

        // Verifica que el NPC esté en el NavMesh
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError("No se encontró una posición válida en el NavMesh para el NPC.");
            }
        }
        agent.speed = patrolSpeed;

        if (npcAudioSource != null && patrolClip != null)
        {
            npcAudioSource.clip = patrolClip;
            npcAudioSource.loop = true;
            npcAudioSource.Play();
        }
    }

    void Update()
    {
        UpdateAnimations();
        // Durante el audio de captura, evitamos cambiar el audio por estado
        switch (currentState)
        {
            case NPCState.Patrolling:
                Patrol();
                // Solo si el switch ya está activado y no se está reproduciendo el audio de captura
                if (!isCapturing && chaseSwitchActivated && CanSeePlayer())
                {
                    currentState = NPCState.Chasing;
                    if (npcAudioSource != null && chaseClip != null)
                    {
                        npcAudioSource.clip = chaseClip;
                        npcAudioSource.loop = true;
                        npcAudioSource.Play();
                    }
                }
                break;

            case NPCState.Chasing:
                agent.speed = chaseSpeed;
                if (CanSeePlayer())
                {
                    // Actualiza el último punto conocido y persigue al jugador
                    lastKnownPlayerPosition = player.position;
                    agent.SetDestination(player.position);
                }
                else
                {
                    // Si pierde la visión, pasa a investigar el último punto donde lo vio
                    currentState = NPCState.Investigating;
                }
                break;

            case NPCState.Investigating:
                agent.speed = chaseSpeed;
                agent.SetDestination(lastKnownPlayerPosition);
                // Si durante la investigación vuelve a ver al jugador, se reanuda la persecución
                if (!isCapturing && CanSeePlayer())
                {
                    currentState = NPCState.Chasing;
                    if (npcAudioSource != null && chaseClip != null && npcAudioSource.clip != chaseClip)
                    {
                        npcAudioSource.clip = chaseClip;
                        npcAudioSource.loop = true;
                        npcAudioSource.Play();
                    }
                }
                // Al llegar al último punto sin detectar al jugador, regresa a patrullar
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    currentState = NPCState.Patrolling;
                    agent.speed = patrolSpeed;
                    if (!isCapturing && npcAudioSource != null && patrolClip != null && npcAudioSource.clip != patrolClip)
                    {
                        npcAudioSource.clip = patrolClip;
                        npcAudioSource.loop = true;
                        npcAudioSource.Play();
                    }
                }
                break;
        }
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("El agente no está en el NavMesh. Intentando reubicarlo.");
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        NavMeshHit hitTarget;
        if (NavMesh.SamplePosition(target.position, out hitTarget, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hitTarget.position);
        }
        else
        {
            Debug.LogWarning("No se encontró un punto válido en el NavMesh para el waypoint.");
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Método que realiza un raycast para determinar si el NPC tiene línea de visión del jugador
    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        // Origen del raycast ajustado a la altura de "ojos" del NPC
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = (player.position - origin).normalized;
        RaycastHit hit;
        // Se define una distancia máxima (por ejemplo, 50 unidades)
        if (Physics.Raycast(origin, direction, out hit, 50f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    // Este método es llamado por el trigger para activar la persecución persistente
    public void ActivateChaseSwitch(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chaseSwitchActivated = true;
            Debug.Log("Switch activado: el NPC perseguirá al jugador de forma persistente.");
            player = other.transform;
            // Si en ese instante se ve al jugador, se fuerza el estado de persecución
            if (CanSeePlayer())
            {
                currentState = NPCState.Chasing;
                if (!isCapturing && npcAudioSource != null && chaseClip != null)
                {
                    // Solo cambia o reproduce si el clip actual no es el de persecución o si no está reproduciendo
                    if (npcAudioSource.clip != chaseClip || !npcAudioSource.isPlaying)
                    {
                        npcAudioSource.clip = chaseClip;
                        npcAudioSource.loop = true;
                        npcAudioSource.Play();
                    }
                }
            }
        }
    }


    // Cuando el NPC colisiona con el jugador, se inicia la secuencia de captura y "respawn"
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && currentState == NPCState.Chasing)
        {
            Debug.Log("Jugador capturado.");
            isCapturing = true;
            if (npcAudioSource != null && captureClip != null)
            {
                npcAudioSource.Stop(); // Detiene cualquier audio que se esté reproduciendo
                npcAudioSource.PlayOneShot(captureClip);
            }

            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.RespawnAtLastCheckpoint();
            }
            else
            {
                Debug.LogWarning("No se encontró PlayerController en el jugador.");
            }
            animator.SetTrigger("NoTrigger"); // Añade esto
            StartCoroutine(TrembleAndReturn());
        }
    }

    // Corrutina para reproducir el temblor y reubicar al NPC (al "capturar" al jugador)
    private IEnumerator TrembleAndReturn()
    {
        // 1. Activar animación "no"
        animator.SetTrigger("NoTrigger");
        agent.isStopped = true;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("No"));

        // 2. Verificar si la animación "no" está activa
        bool isNoAnimationPlaying = animator.GetCurrentAnimatorStateInfo(0).IsName("No");

        // 3. Esperar el audio de captura solo si no está la animación "no"
        if (captureClip != null && !isNoAnimationPlaying)
        {
            yield return new WaitForSeconds(captureClip.length);
        }

        // 4. Configurar posición base
        float originalY = transform.position.y;
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");

        Transform orientation = jugador.transform.Find("Orientation");
        Transform infront = orientation?.Find("Infront");
        Vector3 targetPosition;

        if (infront != null)
        {
            targetPosition = new Vector3(infront.position.x, originalY, infront.position.z);
        }
        else
        {
            Debug.LogWarning("No se encontró 'Infront', usando player.forward");
            Vector3 fallbackPosition = (orientation?.position ?? player.position) + player.forward * transportOffset;
            targetPosition = new Vector3(fallbackPosition.x, originalY, fallbackPosition.z);
        }

        // 5. Ajustar posición al NavMesh
        NavMeshHit navHit;
        Vector3 basePosition = targetPosition;
        if (NavMesh.SamplePosition(targetPosition, out navHit, 2f, NavMesh.AllAreas))
        {
            basePosition = new Vector3(navHit.position.x, originalY, navHit.position.z);
        }
        else
        {
            Debug.LogWarning("Posición no válida en NavMesh");
        }

        // 6. Temblar solo si no hay animación "no"
        if (!isNoAnimationPlaying)
        {
            float trembleDuration = 1f;
            float elapsed = 0f;

            while (elapsed < trembleDuration)
            {
                elapsed += Time.deltaTime;
                Vector3 randomOffset = new Vector3(
                    Random.Range(-0.1f, 0.1f),
                    0,
                    Random.Range(-0.1f, 0.1f)
                );
                agent.Warp(basePosition + randomOffset);
                yield return null;
            }
        }

        // 7. Posicionamiento final
        agent.Warp(basePosition);

        // 8. Reiniciar estado
        currentState = NPCState.Patrolling;
        chaseSwitchActivated = false;
        agent.speed = patrolSpeed;

        if (npcAudioSource != null && patrolClip != null)
        {
            npcAudioSource.clip = patrolClip;
            npcAudioSource.loop = true;
            npcAudioSource.Play();
        }

        isCapturing = false;

        // 9. Forzar actualización de animaciones
        UpdateAnimations();
    }
    private void UpdateAnimations()
    {
        if (animator == null || isNoAnimationPlaying) return;

        // Actualizar Speed según la velocidad del NavMeshAgent
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        // Actualizar State según el estado del NPC
        switch (currentState)
        {
            case NPCState.Patrolling:
            case NPCState.Investigating:
                animator.SetInteger("State", 1); // Walk
                break;
            case NPCState.Chasing:
                animator.SetInteger("State", 2); // Run
                break;
            default:
                animator.SetInteger("State", 0); // Idle
                break;
        }
    }
}
