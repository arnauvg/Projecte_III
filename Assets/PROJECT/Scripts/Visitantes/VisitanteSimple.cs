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

    private Vector3 destinoActual;
    private Vector3 posicionOriginal;
    private float tiempoRebote = 0f;
    private SpriteRenderer spriteRenderer;
    private Vector3 escalaOriginal;
    private bool enMovimiento = false;
    private bool enEscena = false;
    public Transform puntoEntradaEdificio;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        escalaOriginal = transform.localScale;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (enEscena && enMovimiento)
        {
            Vector3 nuevaPos = transform.position;
            float distanciaAlDestino = Mathf.Abs(nuevaPos.x - destinoActual.x);

            if (distanciaAlDestino > 0.05f)
            {
                // Movimiento horizontal
                nuevaPos.x = Mathf.MoveTowards(
                    nuevaPos.x,
                    destinoActual.x,
                    velocidadMovimiento * Time.deltaTime
                );

                // Rebote mientras camina
                tiempoRebote += Time.deltaTime * frecuenciaRebote;
                float offsetY = Mathf.Abs(Mathf.Sin(tiempoRebote)) * alturaRebote;
                nuevaPos.y = posicionOriginal.y + offsetY;

                transform.position = nuevaPos;
            }
            else
            {
                // Llegó al destino
                enMovimiento = false;
                enCentro = true;
                transform.position = new Vector3(
                    transform.position.x,
                    posicionOriginal.y,
                    transform.position.z
                );
                Debug.Log("Visitante llegó al centro");
            }
        }
    }

    public void Aparecer()
    {
        if (puntoEntrada == null || puntoCentro == null)
        {
            Debug.LogError("Puntos de entrada o centro no asignados!");
            return;
        }

        gameObject.SetActive(true);
        transform.position = puntoEntrada.position;
        posicionOriginal = puntoEntrada.position;
        destinoActual = puntoCentro.position;
        enEscena = true;
        enCentro = false;
        enMovimiento = true;
        tiempoRebote = 0f;
        transform.localScale = escalaOriginal;

        Debug.Log("Visitante aparece y camina hacia el centro");
    }

    public void Aceptar()
    {
        if (!enCentro) return;

        if (puntoEntradaEdificio == null)
        {
            Debug.LogError("puntoEntradaEdificio no asignado");
            return;
        }

        destinoActual = puntoEntradaEdificio.position;
        posicionOriginal = transform.position;
        enCentro = false;
        enMovimiento = true;
    }
    public void Rechazar()
    {
        if (!enCentro) return;
        StartCoroutine(Salir(false));
    }

    IEnumerator Salir(bool haciaDerecha)
    {
        enCentro = false;
        float destinoX = haciaDerecha ? puntoCentro.position.x + 12f : puntoCentro.position.x - 12f;

        // Rebote de despedida rápido
        float tiempoDespedida = 0f;
        Vector3 posInicial = transform.position;

        while (tiempoDespedida < 0.2f)
        {
            tiempoDespedida += Time.deltaTime;
            float yOffset = Mathf.Sin(tiempoDespedida * 20f) * 0.1f;
            transform.position = new Vector3(
                transform.position.x,
                posInicial.y + yOffset,
                transform.position.z
            );
            yield return null;
        }

        // Girar sprite si es necesario
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = haciaDerecha;
        }

        // Salir corriendo
        enMovimiento = true;
        while ((haciaDerecha && transform.position.x < destinoX) ||
               (!haciaDerecha && transform.position.x > destinoX))
        {
            float step = velocidadMovimiento * 4f * Time.deltaTime;
            float nuevoX = transform.position.x + (haciaDerecha ? step : -step);
            float yOffset = Mathf.Sin(Time.time * 20f) * 0.05f;
            transform.position = new Vector3(
                nuevoX,
                posicionOriginal.y + yOffset,
                transform.position.z
            );
            yield return null;
        }

        enEscena = false;
        gameObject.SetActive(false);
        Debug.Log("Visitante salió");
    }
}