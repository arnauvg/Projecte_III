using UnityEngine;

public class Recogible : Interactuable
{
    private bool estaRecogido = false;
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private Rigidbody rb;
    private Transform puntoDeSujecion;

    void Start()
    {
        // Guardar posición y rotación original
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        // Crear punto donde se colocará el objeto en la mano (derecha)
        GameObject punto = new GameObject("PuntoSujecion");
        punto.transform.SetParent(Camera.main.transform);
        punto.transform.localPosition = new Vector3(0f, 0f, 1.2f);
        puntoDeSujecion = punto.transform;
    }

    public override bool Recoger()
    {
        if (!estaRecogido)
        {
            estaRecogido = true;
            rb.useGravity = false;
            rb.isKinematic = false;
            return true;
        }
        return false;
    }

    public override void Soltar()
    {
        if (estaRecogido)
        {
            // Lanzar rayo desde el centro de la pantalla
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                // Colocar objeto donde apunta el puntero
                transform.position = hit.point;
            }
            else
            {
                // Si no hay superficie, soltar delante de la cámara
                transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            }

            estaRecogido = false;
            rb.useGravity = true;
            rb.isKinematic = false;

            // Limpiar velocidad
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (estaRecogido)
        {
            // Mover objeto suavemente al punto de sujeción
            Vector3 posicionObjetivo = Vector3.Lerp(transform.position, puntoDeSujecion.position, Time.deltaTime * 40f);
            rb.MovePosition(posicionObjetivo);

            Quaternion rotacionObjetivo = Quaternion.Lerp(transform.rotation, puntoDeSujecion.rotation, Time.deltaTime * 40f);
            rb.MoveRotation(rotacionObjetivo);
        }
    }
}