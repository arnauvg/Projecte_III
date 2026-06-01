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
    public bool yaAtendido = false;
    public VisitanteDatos datosVisitante;

    private Vector3 destinoActual;
    private Vector3 posicionOriginal;
    private float tiempoRebote = 0f;
    private bool enMovimiento = false;
    private Vector3 escalaOriginal;

    [Header("Camuflaje")]
    public SpriteRenderer spriteVisitante;

    private bool camuflajeRevelado = false;
    private GestorVisitantesSimple gestor;
    private bool esBueno = true;

    // Referencia al DialogueManager
    private DialogueManager dialogueManager;
    private bool dialogoMostrado = false;

    void Awake()
    {
        escalaOriginal = transform.localScale;
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    public void ConfigurarVisitante(
        VisitanteDatos datos,
        Transform entrada,
        Transform centro,
        Transform entradaEdificio,
        GestorVisitantesSimple gestorVisitantes)
    {
        datosVisitante = datos;
        esBueno = !datos.esDoble;
        dialogoMostrado = false;

        puntoEntrada = entrada;
        puntoCentro = centro;
        puntoEntradaEdificio = entradaEdificio;
        gestor = gestorVisitantes;

        if (EstadoVisitantes.Instancia != null && EstadoVisitantes.Instancia.HayEstadoGuardado())
        {
            enCentro = EstadoVisitantes.Instancia.visitanteEnCentro;
            yaAtendido = EstadoVisitantes.Instancia.visitanteYaAtendido;

            if (enCentro && !yaAtendido)
            {
                transform.position = puntoCentro.position;
                enMovimiento = false;
                destinoActual = puntoCentro.position;
                posicionOriginal = puntoCentro.position;
            }
            else
            {
                transform.position = puntoEntrada.position;
                enMovimiento = true;
                destinoActual = puntoCentro.position;
                posicionOriginal = puntoEntrada.position;
                enCentro = false;
            }
        }
        else
        {
            enCentro = false;
            yaAtendido = false;
            enMovimiento = true;
            transform.position = puntoEntrada.position;
            destinoActual = puntoCentro.position;
            posicionOriginal = puntoEntrada.position;
        }

        transform.localScale = escalaOriginal;

        if (spriteVisitante != null && datosVisitante != null)
        {
            spriteVisitante.sprite = datosVisitante.spriteNormal;
        }

        Debug.Log($"Visitante: {datosVisitante.nombreVisitante} - {(esBueno ? "BUENO" : "MALO")}");
    }

    void Update()
    {
        if (!enMovimiento) return;
        if (yaAtendido) return;

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

            if (!yaAtendido && !enCentro)
            {
                enCentro = true;
                Debug.Log("Visitante llegó al centro");

                // Mostrar diálogo de bienvenida
                MostrarDialogoBienvenida();

                EstadoVisitantes.Instancia?.GuardarEstadoVisitante(this);
            }
        }
    }

    void MostrarDialogoBienvenida()
    {
        if (dialogoMostrado) return;
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
            if (dialogueManager == null)
            {
                Debug.LogWarning("DialogueManager no encontrado");
                return;
            }
        }

        if (!string.IsNullOrEmpty(datosVisitante.dialogoBienvenida))
        {
            dialogoMostrado = true;
            dialogueManager.MostrarDialogoSimple(datosVisitante.nombreVisitante, datosVisitante.dialogoBienvenida);
            Debug.Log($"📢 {datosVisitante.nombreVisitante}: {datosVisitante.dialogoBienvenida}");
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

        Debug.Log($"Visitante revelado con {reveladorUsado}: era un doble");
    }

    public void Aceptar()
    {
        if (!enCentro || yaAtendido) return;

        Debug.Log("=== ACEPTAR VISITANTE ===");

        yaAtendido = true;
        enCentro = false;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
        {
            if (esBueno)
                gestion.RegistrarAcierto();
            else
                gestion.RegistrarFallo();
        }

        if (puntoEntradaEdificio != null)
        {
            StartCoroutine(MoverHacia(puntoEntradaEdificio.position));
        }

        EstadoVisitantes.Instancia?.GuardarEstadoVisitante(this);
    }

    public void Rechazar()
    {
        if (!enCentro || yaAtendido) return;

        Debug.Log("=== RECHAZAR VISITANTE ===");

        yaAtendido = true;
        enCentro = false;

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
        {
            if (!esBueno)
                gestion.RegistrarAcierto();
            else
                gestion.RegistrarFallo();
        }

        if (puntoEntrada != null)
        {
            StartCoroutine(MoverHacia(puntoEntrada.position));
        }

        EstadoVisitantes.Instancia?.GuardarEstadoVisitante(this);
    }

    IEnumerator MoverHacia(Vector3 destino)
    {
        Debug.Log($"🚶 Visitante moviéndose desde {transform.position} hacia {destino}");

        float yBase = transform.position.y;
        float tiempoReboteLocal = 0f;

        while (Vector3.Distance(transform.position, destino) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);

            tiempoReboteLocal += Time.deltaTime * frecuenciaRebote;
            float offsetY = Mathf.Abs(Mathf.Sin(tiempoReboteLocal)) * alturaRebote;
            Vector3 pos = transform.position;
            pos.y = yBase + offsetY;
            transform.position = pos;

            yield return null;
        }

        transform.position = destino;
        Debug.Log("✅ Visitante llegó al destino");

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
        dialogoMostrado = false;

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