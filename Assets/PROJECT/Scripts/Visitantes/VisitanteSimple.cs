using UnityEngine;
using System.Collections;

public class VisitanteSimple : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform puntoEntrada;
    public Transform puntoCentro;
    public Transform puntoEntradaEdificio;
    public float velocidadMovimiento = 1.5f;
    public float alturaRebote = 0.1f;
    public float frecuenciaRebote = 10f;

    [Header("Estado")]
    public bool enCentro = false;

    private Vector3 destinoActual;
    private Vector3 posicionOriginal;
    private float tiempoRebote = 0f;
    private bool enMovimiento = false;
    private bool yaAtendido = false;
    private Vector3 escalaOriginal;

    [Header("Camuflaje")]
    public SpriteRenderer spriteVisitante;
    public Sprite spriteNormal;
    public Sprite spriteRevelado;
    private bool camuflajeRevelado = false;

    void Awake()
    {
        escalaOriginal = transform.localScale;
    }

    void Start()
    {
        transform.localScale = escalaOriginal;
        transform.position = puntoEntrada.position;
        destinoActual = puntoCentro.position;
        posicionOriginal = puntoEntrada.position;
        enMovimiento = true;
        yaAtendido = false;
        Debug.Log("Visitante aparece desde la entrada");
    }

    void Update()
    {
        if (!enMovimiento) return;

        Vector3 nuevaPos = transform.position;
        float distancia = Mathf.Abs(nuevaPos.x - destinoActual.x);

        if (distancia > 0.05f)
        {
            nuevaPos.x = Mathf.MoveTowards(nuevaPos.x, destinoActual.x, velocidadMovimiento * Time.deltaTime);
            tiempoRebote += Time.deltaTime * frecuenciaRebote;
            float offsetY = Mathf.Abs(Mathf.Sin(tiempoRebote)) * alturaRebote;
            nuevaPos.y = posicionOriginal.y + offsetY;
            transform.position = nuevaPos;
        }
        else
        {
            enMovimiento = false;
            enCentro = true;
            transform.position = new Vector3(destinoActual.x, posicionOriginal.y, transform.position.z);
            Debug.Log("Visitante llegó al centro - Esperando decisión");
        }
    }
    public void RevelarCamuflado()
    {
        if (camuflajeRevelado) return;

        camuflajeRevelado = true;

        if (spriteVisitante != null && spriteRevelado != null)
        {
            spriteVisitante.sprite = spriteRevelado;
        }

        Debug.Log("El visitante ha sido revelado: estaba camuflado");
    }

    public void Aceptar()
    {
        if (!enCentro || yaAtendido) return;

        yaAtendido = true;
        enCentro = false;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
            gestion.RegistrarVisitanteAceptado();

        Debug.Log("Visitante aceptado - Saliendo por la derecha");

        destinoActual = puntoEntradaEdificio.position;
        posicionOriginal = transform.position;
        enMovimiento = true;

        // Cuando termine de moverse, notificar fin de movimiento
        StartCoroutine(EsperarFinMovimiento());
    }

    public void Rechazar()
    {
        if (!enCentro || yaAtendido) return;

        yaAtendido = true;
        enCentro = false;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
            gestion.RegistrarVisitanteRechazado();

        Debug.Log("Visitante rechazado - Saliendo por la izquierda");

        destinoActual = puntoEntrada.position;
        posicionOriginal = transform.position;
        enMovimiento = true;

        // Cuando termine de moverse, notificar fin de movimiento
        StartCoroutine(EsperarFinMovimiento());
    }

    IEnumerator EsperarFinMovimiento()
    {
        // Esperar mientras se mueve
        while (enMovimiento)
        {
            yield return null;
        }

        // Un pequeño retraso después de llegar al destino
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Visitante terminó de salir - Fin de la noche");

        // Notificar al gestor que el visitante terminó
        GestorVisitantesSimple gestor = FindFirstObjectByType<GestorVisitantesSimple>();
        if (gestor != null)
            gestor.VisitanteTerminoSalir();
    }

    public void ReiniciarParaNuevaNoche()
    {
        yaAtendido = false;
        enCentro = false;
        enMovimiento = true;
        transform.position = puntoEntrada.position;
        destinoActual = puntoCentro.position;
        posicionOriginal = puntoEntrada.position;
        transform.localScale = escalaOriginal;

        camuflajeRevelado = false;

        if (spriteVisitante != null && spriteNormal != null)
        {
            spriteVisitante.sprite = spriteNormal;
        }

        Debug.Log("Visitante reiniciado para nueva noche");
    }
}

