using UnityEngine;
using System.Collections;

public class Recogible : Interactuable
{
    [Header("Posición en mano")]
    public Vector3 posicionEnMano = new Vector3(0f, 0f, 0.6f);

    [Header("Auto-emparentado")]
    public float radioDeteccion = 0.5f;

    [Header("Posición dentro del cajón")]
    public Vector3 posicionLocalEnCajon = new Vector3(0f, 0f, 0f);
    public Vector3 rotacionLocalEnCajon = new Vector3(0f, 0f, 0f);

    [Header("Tipo de revelador")]
    public TipoRevelador tipoRevelador = TipoRevelador.Ninguno;

    public static Recogible objetoEnMano;

    private bool estaRecogido = false;
    private Rigidbody rb;
    private Transform puntoMano;
    private Collider miCollider;
    private Transform cajonPadre;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private Transform padreInicial;

    void Start()
    {
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
        padreInicial = transform.parent;

        // Guardar el cajón padre (si tiene)
        cajonPadre = transform.parent;

        // Si ya es hijo de un cajón, aplicar posición y rotación
        if (cajonPadre != null && cajonPadre.CompareTag("Cajon"))
        {
            transform.localPosition = posicionLocalEnCajon;
            transform.localRotation = Quaternion.Euler(rotacionLocalEnCajon);
        }

        // Configurar Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Guardar collider
        miCollider = GetComponent<Collider>();

        // Crear punto en la mano
        GameObject punto = new GameObject("PuntoMano");
        punto.transform.SetParent(Camera.main.transform);
        punto.transform.localPosition = posicionEnMano;
        puntoMano = punto.transform;
    }

    public override bool Recoger()
    {
        if (!estaRecogido)
        {
            estaRecogido = true;
            objetoEnMano = this;
            transform.SetParent(null); // Desemparentar
            miCollider.enabled = false; // Desactivar collider
            rb.isKinematic = false;
            rb.useGravity = false;
            return true;
        }
        return false;
    }

    public override void Soltar()
    {
        if (estaRecogido)
        {
            estaRecogido = false;

            if (objetoEnMano == this)
                objetoEnMano = null;

            // Lanzar rayo para soltar
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
                transform.position = hit.point;
            else
                transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;

            estaRecogido = false;

            // Volver a su padre original
            transform.SetParent(padreInicial);

            // Volver a su posición y rotación inicial
            transform.position = posicionInicial;
            transform.rotation = rotacionInicial;

            miCollider.enabled = true; // Reactivar collider
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Intentar volver a ser hijo de un cajón
            BuscarYEmparentarConCajon();
        }
    }

    void BuscarYEmparentarConCajon()
    {
        // Buscar todos los colliders cerca
        Collider[] objetosCerca = Physics.OverlapSphere(transform.position, radioDeteccion);

        foreach (Collider col in objetosCerca)
        {
            // Buscar si tiene tag "Cajon"
            if (col.CompareTag("Cajon"))
            {
                // Ser hijo del cajón
                transform.SetParent(col.transform);

                // Aplicar posición y rotación específicas dentro del cajón
                transform.localPosition = posicionLocalEnCajon;
                transform.localRotation = Quaternion.Euler(rotacionLocalEnCajon);

                // Congelar el objeto
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                Debug.Log($"{gameObject.name} guardado en {col.name} en posición {posicionLocalEnCajon}");
                return;
            }
        }
    }

    void Update()
    {
        if (estaRecogido)
        {
            // Seguir la mano
            transform.position = Vector3.Lerp(transform.position, puntoMano.position, Time.deltaTime * 40f);
            transform.rotation = Quaternion.Lerp(transform.rotation, puntoMano.rotation, Time.deltaTime * 40f);
        }
    }

    // 👇 NUEVA FUNCIÓN: Verifica si el objeto está dentro de un cajón cerrado
    public bool EstaEnCajonCerrado()
    {
        // Si no tiene padre, no está en ningún cajón
        if (transform.parent == null) return false;

        // Buscar el cajón padre
        Transform posibleCajon = transform.parent;
        while (posibleCajon != null)
        {
            if (posibleCajon.CompareTag("Cajon"))
            {
                // Verificar si el cajón tiene el componente Cajon y está cerrado
                Cajon cajon = posibleCajon.GetComponent<Cajon>();
                if (cajon != null)
                {
                    return !cajon.EstaAbierto(); // Está cerrado -> true
                }
                return true; // Si no tiene script Cajon, asumimos que está cerrado
            }
            posibleCajon = posibleCajon.parent;
        }

        return false;
    }

    // Para visualizar el radio en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}