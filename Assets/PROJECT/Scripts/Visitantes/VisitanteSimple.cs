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

    private bool camuflajeRevelado = false;
    private VisitanteDatos datosVisitante;
    private GestorVisitantesSimple gestor;

    // ✅ NUEVA VARIABLE
    private bool esBueno = true;

    void Awake()
    {
        escalaOriginal = transform.localScale;
    }

    public void ConfigurarVisitante(
        VisitanteDatos datos,
        Transform entrada,
        Transform centro,
        Transform entradaEdificio,
        GestorVisitantesSimple gestorVisitantes
    )
    {
        datosVisitante = datos;

        // ✅ CONFIGURAR SI ES BUENO O MALO
        // Si esDoble == true → es MALO (false)
        // Si esDoble == false → es BUENO (true)
        esBueno = !datos.esDoble;

        puntoEntrada = entrada;
        puntoCentro = centro;
        puntoEntradaEdificio = entradaEdificio;
        gestor = gestorVisitantes;

        yaAtendido = false;
        enCentro = false;
        camuflajeRevelado = false;
        enMovimiento = true;

        transform.localScale = escalaOriginal;
        transform.position = puntoEntrada.position;

        destinoActual = puntoCentro.position;
        posicionOriginal = puntoEntrada.position;

        if (spriteVisitante != null && datosVisitante != null)
        {
            spriteVisitante.sprite = datosVisitante.spriteNormal;
        }

        Debug.Log($"Aparece visitante: {datosVisitante.nombreVisitante} - {(esBueno ? "BUENO" : "MALO")}");
    }

    // En Update(), asegura que cuando llega al centro se establece bien:
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
            transform.position = new Vector3(destinoActual.x, posicionOriginal.y, transform.position.z);

            if (!yaAtendido)
            {
                enCentro = true;
                Debug.Log("Visitante llegó al centro - Esperando decisión");
            }
        }
    }

    public void RevelarCamuflado(TipoRevelador reveladorUsado)
    {
        if (camuflajeRevelado) return;
        if (datosVisitante == null) return;

        if (!datosVisitante.esDoble)
        {
            Debug.Log("Este visitante no es un doble.");
            return;
        }

        if (reveladorUsado != datosVisitante.reveladorNecesario)
        {
            Debug.Log($"Objeto incorrecto. Este visitante no se revela con {reveladorUsado}");
            return;
        }

        camuflajeRevelado = true;

        if (spriteVisitante != null && datosVisitante.spriteRevelado != null)
        {
            spriteVisitante.sprite = datosVisitante.spriteRevelado;
        }

        Debug.Log($"El visitante ha sido revelado con {reveladorUsado}: era un doble");
    }

    public void Aceptar()
    {
        if (!enCentro || yaAtendido) return;

        yaAtendido = true;
        enCentro = false;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();

        if (gestion != null)
        {
            if (esBueno)
                gestion.RegistrarAcierto();  // Bueno aceptado = acierto
            else
                gestion.RegistrarFallo();    // Malo aceptado = fallo
        }

        destinoActual = puntoEntradaEdificio.position;
        posicionOriginal = transform.position;
        enMovimiento = true;

        StartCoroutine(EsperarFinMovimiento());
    }

    public void Rechazar()
    {
        if (!enCentro || yaAtendido) return;

        yaAtendido = true;
        enCentro = false;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();

        if (gestion != null)
        {
            if (!esBueno)
                gestion.RegistrarAcierto();  // Malo rechazado = acierto
            else
                gestion.RegistrarFallo();    // Bueno rechazado = fallo
        }

        destinoActual = puntoEntrada.position;
        posicionOriginal = transform.position;
        enMovimiento = true;

        StartCoroutine(EsperarFinMovimiento());
    }

    IEnumerator EsperarFinMovimiento()
    {
        while (enMovimiento)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        if (gestor != null)
        {
            gestor.VisitanteTerminoSalir();
        }

        Destroy(gameObject);
    }

    public void ReiniciarParaNuevaNoche()
    {
        yaAtendido = false;
        enCentro = false;
        camuflajeRevelado = false;
        enMovimiento = false;

        if (puntoEntrada != null)
        {
            transform.position = puntoEntrada.position;
        }

        if (datosVisitante != null && spriteVisitante != null)
        {
            spriteVisitante.sprite = datosVisitante.spriteNormal;
        }
    }
}