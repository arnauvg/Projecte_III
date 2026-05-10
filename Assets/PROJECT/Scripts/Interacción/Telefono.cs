using UnityEngine;

public class Telefono : Interactuable
{
    private bool enMano = false;
    private Vector3 posOriginal;
    private Quaternion rotOriginal;
    private Transform puntoMano;
    private Rigidbody rb;

    void Start()
    {
        posOriginal = transform.position;
        rotOriginal = transform.rotation;
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        GameObject punto = new GameObject("PuntoTelefono");
        punto.transform.SetParent(Camera.main.transform);
        punto.transform.localPosition = new Vector3(-0.35f, -0.1f, 0.5f);
        punto.transform.localRotation = Quaternion.Euler(15f, 30f, 0f);
        puntoMano = punto.transform;
    }

    public override bool Recoger()
    {
        if (!enMano)
        {
            enMano = true;
            rb.isKinematic = true;
            rb.useGravity = false;
            return true;
        }
        return false;
    }

    public override void Soltar()
    {
        if (enMano)
        {
            transform.position = posOriginal;
            transform.rotation = rotOriginal;
            enMano = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (enMano)
        {
            transform.position = Vector3.Lerp(transform.position, puntoMano.position, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, puntoMano.rotation, Time.deltaTime * 15f);
        }
    }
}