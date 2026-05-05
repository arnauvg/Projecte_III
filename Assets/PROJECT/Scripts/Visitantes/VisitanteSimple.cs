using UnityEngine;

public class VisitanteSimple : MonoBehaviour
{
    public Transform puntoEntrada;
    public Transform puntoCentro;
    public Transform puntoEntradaEdificio;

    public float velocidadMovimiento = 1.5f;
    public float alturaRebote = 0.1f;
    public float frecuenciaRebote = 10f;

    public bool enCentro = false;

    private Vector3 destinoActual;
    private Vector3 posicionOriginal;
    private float tiempoRebote = 0f;
    private Vector3 escalaOriginal;

    private bool enMovimiento = false;
    private bool enEscena = false;
    private bool decisionTomada = false;
    private bool desaparecerAlLlegar = false;

    void Start()
    {
        escalaOriginal = transform.localScale;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!enEscena || !enMovimiento) return;

        Vector3 nuevaPos = transform.position;
        float distancia = Mathf.Abs(nuevaPos.x - destinoActual.x);

        if (distancia > 0.05f)
        {
            nuevaPos.x = Mathf.MoveTowards(
                nuevaPos.x,
                destinoActual.x,
                velocidadMovimiento * Time.deltaTime
            );

            tiempoRebote += Time.deltaTime * frecuenciaRebote;
            float offsetY = Mathf.Abs(Mathf.Sin(tiempoRebote)) * alturaRebote;
            nuevaPos.y = posicionOriginal.y + offsetY;

            transform.position = nuevaPos;
        }
        else
        {
            LlegarDestino();
        }
    }

    void LlegarDestino()
    {
        enMovimiento = false;

        transform.position = new Vector3(
            destinoActual.x,
            posicionOriginal.y,
            transform.position.z
        );

        if (desaparecerAlLlegar)
        {
            enEscena = false;
            enCentro = false;
            decisionTomada = false;
            desaparecerAlLlegar = false;
            gameObject.SetActive(false);
            return;
        }

        enCentro = true;
        Debug.Log("Visitante llegó al centro");
    }

    public void Aparecer()
    {
        if (puntoEntrada == null || puntoCentro == null)
        {
            Debug.LogError("Puntos no asignados");
            return;
        }

        gameObject.SetActive(true);

        transform.position = puntoEntrada.position;
        posicionOriginal = puntoEntrada.position;
        destinoActual = puntoCentro.position;

        enEscena = true;
        enCentro = false;
        enMovimiento = true;
        decisionTomada = false;
        desaparecerAlLlegar = false;
        tiempoRebote = 0f;
        transform.localScale = escalaOriginal;
    }

    public void Aceptar()
    {
        if (!enCentro || decisionTomada) return;

        if (puntoEntradaEdificio == null)
        {
            Debug.LogError("puntoEntradaEdificio no asignado");
            return;
        }

        decisionTomada = true;
        enCentro = false;

        destinoActual = puntoEntradaEdificio.position;
        posicionOriginal = transform.position;
        desaparecerAlLlegar = true;
        enMovimiento = true;
    }

    public void Rechazar()
    {
        if (!enCentro || decisionTomada) return;

        if (puntoEntrada == null)
        {
            Debug.LogError("puntoEntrada no asignado");
            return;
        }

        decisionTomada = true;
        enCentro = false;

        destinoActual = puntoEntrada.position;
        posicionOriginal = transform.position;
        desaparecerAlLlegar = true;
        enMovimiento = true;
    }
}