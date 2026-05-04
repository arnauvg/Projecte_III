using UnityEngine;
using System.Collections;

public class VisitanteSimple : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform puntoEntrada;
    public Transform puntoCentro;
    public float velocidadMovimiento = 1.5f;

    [Header("Rebote mientras camina")]
    public float alturaRebote = 0.1f;
    public float frecuenciaRebote = 10f;

    [Header("Estados")]
    public bool enCentro = false;
    public bool estaEnEscena = false;
    public bool haSidoAtendido = false;

    [Header("Referencias")]
    public Transform puntoEntradaEdificio;

    private Vector3 destinoActual;
    private Vector3 posicionOriginal;
    private float tiempoRebote = 0f;
    private SpriteRenderer spriteRenderer;
    private Vector3 escalaOriginal;
    private bool enMovimiento = false;

    private static VisitanteSimple instancia;

    void Awake()
    {
        // Si ya existe una instancia y no soy yo, destruirme
        if (instancia != null && instancia != this)
        {
            Debug.Log("Vampirikiki duplicado destruido - Ya existe una instancia");
            Destroy(gameObject);
            return;
        }

        // Asegurar que el objeto es raíz
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        // Soy la instancia principal
        instancia = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("Vampirikiki configurado como persistente (instancia única)");
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        escalaOriginal = transform.localScale;

        if (!estaEnEscena)
        {
            gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Si soy la instancia principal y me destruyen, limpiar la referencia
        if (instancia == this)
        {
            instancia = null;
            Debug.Log("Vampirikiki: Instancia principal destruida");
        }
    }

    void Update()
    {
        if (estaEnEscena && enMovimiento)
        {
            Vector3 nuevaPos = transform.position;
            float distanciaAlDestino = Mathf.Abs(nuevaPos.x - destinoActual.x);

            if (distanciaAlDestino > 0.05f)
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
                transform.position = new Vector3(transform.position.x, posicionOriginal.y, transform.position.z);
                Debug.Log("Visitante llegó al centro");
            }
        }
    }

    public void Aparecer()
    {
        if (haSidoAtendido)
        {
            Debug.Log("Visitante ya fue atendido esta noche, no aparece de nuevo");
            return;
        }

        if (puntoEntrada == null || puntoCentro == null)
        {
            Debug.LogError("Puntos de entrada o centro no asignados!");
            return;
        }

        gameObject.SetActive(true);
        transform.position = puntoEntrada.position;
        posicionOriginal = puntoEntrada.position;
        destinoActual = puntoCentro.position;
        estaEnEscena = true;
        enCentro = false;
        enMovimiento = true;
        haSidoAtendido = false;
        tiempoRebote = 0f;
        transform.localScale = escalaOriginal;

        Debug.Log("Visitante aparece y camina hacia el centro");
    }

    public void Aceptar()
    {
        if (!enCentro)
        {
            Debug.Log("No se puede aceptar: visitante no está en el centro");
            return;
        }
        if (haSidoAtendido)
        {
            Debug.Log("Visitante ya fue atendido");
            return;
        }

        haSidoAtendido = true;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
        {
            gestion.RegistrarVisitanteAceptado();
            Debug.Log("Visitante ACEPTADO - Registrado en sistema");
        }

        if (puntoEntradaEdificio == null)
        {
            Debug.LogError("puntoEntradaEdificio no asignado");
            return;
        }

        destinoActual = puntoEntradaEdificio.position;
        posicionOriginal = transform.position;
        enCentro = false;
        enMovimiento = true;

        StartCoroutine(DesactivarDespuesDeSalir());
    }

    public void Rechazar()
    {
        if (!enCentro)
        {
            Debug.Log("No se puede rechazar: visitante no está en el centro");
            return;
        }
        if (haSidoAtendido)
        {
            Debug.Log("Visitante ya fue atendido");
            return;
        }

        haSidoAtendido = true;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
        {
            gestion.RegistrarVisitanteRechazado();
            Debug.Log("Visitante RECHAZADO - Registrado en sistema");
        }

        if (puntoEntrada == null)
        {
            Debug.LogError("puntoEntrada no asignado");
            return;
        }

        destinoActual = puntoEntrada.position;
        posicionOriginal = transform.position;
        enCentro = false;
        enMovimiento = true;

        StartCoroutine(DesactivarDespuesDeSalir());
    }

    IEnumerator DesactivarDespuesDeSalir()
    {
        yield return new WaitForSeconds(2f);
        estaEnEscena = false;
        gameObject.SetActive(false);
        Debug.Log("Visitante desactivado después de salir");
    }

    public void ReiniciarParaNuevaNoche()
    {
        haSidoAtendido = false;
        estaEnEscena = false;
        gameObject.SetActive(false);
        Debug.Log("Visitante reiniciado para nueva noche");
    }

    // Método para verificar si el objeto sigue vivo
    public bool EstaVivo()
    {
        return this != null && gameObject != null;
    }
}