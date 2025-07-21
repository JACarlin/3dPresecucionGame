using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public static FirstPersonMovement Instance { get; private set; }

    public float sensX = 200f;
    public float sensY = 200f;

    public Transform orientation;

    float xRotation;
    float yRotation;

    public static bool inventoryOpen = false; // Si el inventario está abierto, no se mueve la cámara.

    // Variables para modo de restricción
    private bool cameraRestricted = false;
    private Quaternion restrictedTargetRotation; // Rotación base para mirar el objeto (entrada del armario)
    private float restrictedXOffset = 0f;
    private float restrictedYOffset = 0f;
    public float restrictedMaxAngleOffset = 10f; // Máximo ángulo de offset permitido en grados

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (inventoryOpen) return;

        if (cameraRestricted)
        {
            // Permitir una modificación limitada a partir de la rotación base
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            restrictedYOffset += mouseX;
            restrictedXOffset -= mouseY;  // Se resta porque la convención es similar a la rotación normal

            // Limitar los offsets a un rango máximo
            restrictedYOffset = Mathf.Clamp(restrictedYOffset, -restrictedMaxAngleOffset, restrictedMaxAngleOffset);
            restrictedXOffset = Mathf.Clamp(restrictedXOffset, -restrictedMaxAngleOffset, restrictedMaxAngleOffset);

            // Se suma el offset a la rotación base
            Vector3 baseEuler = restrictedTargetRotation.eulerAngles;
            float newX = baseEuler.x + restrictedXOffset;
            float newY = baseEuler.y + restrictedYOffset;

            transform.rotation = Quaternion.Euler(newX, newY, 0);
            orientation.rotation = Quaternion.Euler(0, newY, 0);
        }
        else
        {
            // Movimiento normal de cámara
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    /// <summary>
    /// Activa la restricción de cámara, fijando la rotación base para que el jugador mire al objeto dado (por ejemplo, la entrada del armario).
    /// </summary>
    /// <param name="targetLookAt">Transform del objeto al que se quiere mirar (la entrada del armario).</param>
    public void ApplyCameraRestriction(Transform targetLookAt)
    {
        cameraRestricted = true;
        // Reiniciar los offsets
        restrictedXOffset = 0f;
        restrictedYOffset = 0f;
        // Calcular la rotación base para mirar el objeto indicado.
        Vector3 direction = (targetLookAt.position - transform.position).normalized;
        restrictedTargetRotation = Quaternion.LookRotation(direction);
        // Establecer la rotación de la cámara inmediatamente.
        transform.rotation = restrictedTargetRotation;
        orientation.rotation = Quaternion.Euler(0, restrictedTargetRotation.eulerAngles.y, 0);
    }

    /// <summary>
    /// Desactiva la restricción de cámara, permitiendo el movimiento normal.
    /// </summary>
    public void RemoveCameraRestriction()
    {
        cameraRestricted = false;
        // Se actualizan las variables de rotación para que el movimiento continúe de forma fluida
        Vector3 euler = transform.rotation.eulerAngles;
        xRotation = euler.x;
        yRotation = euler.y;
    }
}
