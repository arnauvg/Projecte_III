using UnityEngine;

public class MouseLook360 : MonoBehaviour
{
    [Header("Sensibilidad (valores recomendados: 1-10)")]
    public float mouseSensitivity = 5f;

    [Header("Límites verticales")]
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    [Header("Suavizado (opcional, reduce glitches)")]
    public bool useSmoothing = true;
    public float smoothTime = 0.03f;

    private float verticalRotation = 0f;   // Ángulo actual en X (arriba/abajo)
    private float horizontalRotation = 0f; // Ángulo actual en Y (izquierda/derecha)

    // Variables para suavizado
    private float currentVerticalAngle;
    private float currentHorizontalAngle;
    private float verticalVelocity;
    private float horizontalVelocity;

    void Start()
    {
        // Bloquear cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializar rotaciones a partir de la rotación actual del objeto
        Vector3 currentRot = transform.eulerAngles;
        horizontalRotation = currentRot.y;
        verticalRotation = currentRot.x;

        // Ajustar si el ángulo vertical supera 180 (para trabajar con ángulos negativos)
        if (verticalRotation > 180) verticalRotation -= 360;

        currentHorizontalAngle = horizontalRotation;
        currentVerticalAngle = verticalRotation;
    }

    void Update()
    {
        // Entrada del ratón (sin Time.deltaTime, queremos respuesta directa)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Actualizar rotaciones objetivo
        horizontalRotation += mouseX;

        verticalRotation -= mouseY; // Restamos para que mover ratón arriba mire arriba
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // Aplicar rotación con o sin suavizado
        if (useSmoothing)
        {
            // Suavizado con SmoothDamp para evitar saltos bruscos
            currentHorizontalAngle = Mathf.SmoothDamp(
                currentHorizontalAngle,
                horizontalRotation,
                ref horizontalVelocity,
                smoothTime
            );

            currentVerticalAngle = Mathf.SmoothDamp(
                currentVerticalAngle,
                verticalRotation,
                ref verticalVelocity,
                smoothTime
            );

            transform.rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        }
    }

    // Opcional: resetear rotación si se desea
    public void ResetRotation()
    {
        horizontalRotation = 0f;
        verticalRotation = 0f;
        currentHorizontalAngle = 0f;
        currentVerticalAngle = 0f;
        transform.rotation = Quaternion.identity;
    }
}