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

    void Awake()
    {
        escalaOriginal = transform.localScale;
    }

    void Start()
    {
        // El visitante siempre visible desde el inicio
        transform.localScale = escalaOriginal;
        transform.position = puntoEntrada.position;
        destinoActual = puntoCentro.position;
        posicionOriginal = puntoEntrada.position;
        enMovimiento = true;
        yaAtendido = false;
        Debug.Log("Visitante listo en la entrada");
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
            Debug.Log("🦇 Visitante llegó al centro");
        }
    }

    public void Aceptar()
    {
        if (!enCentro || yaAtendido) return;

        yaAtendido = true;
        enCentro = false;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
            gestion.RegistrarVisitanteAceptado();

        Debug.Log("❌ Has dejado pasar al vampiro! -50€");

        destinoActual = puntoEntradaEdificio.position;
        posicionOriginal = transform.position;
        enMovimiento = true;
    }

    public void Rechazar()
    {
        if (!enCentro || yaAtendido) return;

        yaAtendido = true;
        enCentro = false;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
            gestion.RegistrarVisitanteRechazado();

        Debug.Log("✅ Has rechazado al vampiro! Bien hecho");

        destinoActual = puntoEntrada.position;
        posicionOriginal = transform.position;
        enMovimiento = true;
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
        Debug.Log("Visitante reiniciado para nueva noche");
    }
}