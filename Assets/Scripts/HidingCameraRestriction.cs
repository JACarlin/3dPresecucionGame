using UnityEngine;

public class HidingCameraRestriction : MonoBehaviour
{
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float minPitch = -20f;
    public float maxPitch = 20f;
    private float currentPitch = 0f;

    void Update()
    {
        // Permite únicamente movimiento vertical (pitch) y un muy leve movimiento horizontal (yaw)
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        // Movimiento horizontal muy restringido
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * 0.1f; // Solo el 10% del movimiento normal

        // Aplicar la rotación al transform local de la cámara.
        transform.localEulerAngles = new Vector3(currentPitch, mouseX, 0f);
    }
}
