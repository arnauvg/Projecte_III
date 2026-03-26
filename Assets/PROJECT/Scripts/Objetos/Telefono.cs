using UnityEngine;

public class Telefono : Interactuable
{
    private bool estaEnMano = false;
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private Transform puntoTelefono;
    private Rigidbody rb;

    void Start()
    {
        // Guardar posición y rotación original
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;

        // Obtener el Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Crear punto donde se colocará el teléfono al hablar (LADO IZQUIERDO)
        GameObject punto = new GameObject("PuntoTelefono");
        punto.transform.SetParent(Camera.main.transform);
        punto.transform.localPosition = new Vector3(-0.35f, -0.1f, 0.5f);
        punto.transform.localRotation = Quaternion.Euler(15f, 30f, 0f);
        puntoTelefono = punto.transform;
    }

    public override bool Recoger()
    {
        if (!estaEnMano)
        {
            estaEnMano = true;
            rb.isKinematic = true;
            rb.useGravity = false;
            return true;
        }
        return false;
    }

    public override void Soltar()
    {
        if (estaEnMano)
        {
            // Volver a posición original
            transform.position = posicionOriginal;
            transform.rotation = rotacionOriginal;
            estaEnMano = false;
            rb.isKinematic = false;
            rb.useGravity = true;

            // Limpiar velocidades
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (estaEnMano)
        {
            // Mover suavemente al punto del teléfono
            transform.position = Vector3.Lerp(transform.position, puntoTelefono.position, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, puntoTelefono.rotation, Time.deltaTime * 15f);
        }
    }
}