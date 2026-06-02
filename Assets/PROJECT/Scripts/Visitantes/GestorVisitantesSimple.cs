using UnityEngine;
using System.Collections;

public class GestorVisitantesSimple : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject prefabVisitante;

    [Header("Puntos de movimiento")]
    public Transform puntoEntrada;
    public Transform puntoCentro;
    public Transform puntoEntradaEdificio;

    [Header("Tiempo")]
    public float tiempoEntreVisitantes = 3f;

    [Header("Límite de visitantes por noche")]
    public int maxVisitantesPorNoche = 3;

    private VisitanteSimple visitanteActual;
    private bool esperandoVisitante = false;
    private int visitantesAtendidosEnNoche = 0;
    private bool nocheTerminada = false;
    private bool generacionVisitantesPausada = true;
    private bool tutorialCompletado = false;
    private bool pendingVisitante = false;

    void Start()
    {
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        generacionVisitantesPausada = true;
        tutorialCompletado = false;
        pendingVisitante = false;

        DialogueManager.OnTutorialEnded += TutorialCompletado;

        Debug.Log("GestorVisitantesSimple: Esperando fin del tutorial...");
    }

    void OnDestroy()
    {
        DialogueManager.OnTutorialEnded -= TutorialCompletado;
    }

    void TutorialCompletado()
    {
        if (tutorialCompletado) return;

        tutorialCompletado = true;
        generacionVisitantesPausada = false;
        Debug.Log("📞 Tutorial completado - Comienzan a llegar visitantes");

        StartCoroutine(CrearVisitanteConDelay());
    }

    IEnumerator CrearVisitanteConDelay()
    {
        yield return new WaitForSeconds(0.5f);
        CrearVisitanteActual();
    }

    void CrearVisitanteActual()
    {
        Debug.Log($"CrearVisitanteActual - pausada={generacionVisitantesPausada}, atendidos={visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");

        if (generacionVisitantesPausada)
        {
            pendingVisitante = true;
            Debug.Log("📌 Generación pausada, visitante pendiente");
            return;
        }
        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log("Máximo de visitantes alcanzado");
            return;
        }
        if (nocheTerminada)
        {
            Debug.Log("Noche terminada");
            return;
        }
        if (EstadoVisitantes.Instancia == null)
        {
            Debug.LogError("EstadoVisitantes.Instancia es null");
            return;
        }

        pendingVisitante = false;

        if (EstadoVisitantes.Instancia.VisitanteYaAtendido())
        {
            EstadoVisitantes.Instancia.PasarAlSiguienteVisitante();
            EstadoVisitantes.Instancia.LimpiarEstadoGuardado();
        }

        VisitanteDatos datos = EstadoVisitantes.Instancia.ObtenerVisitanteActual();
        if (datos == null)
        {
            Debug.Log("No hay más visitantes");
            return;
        }

        GameObject nuevoVisitante = Instantiate(prefabVisitante);
        visitanteActual = nuevoVisitante.GetComponent<VisitanteSimple>();
        visitanteActual.ConfigurarVisitante(datos, puntoEntrada, puntoCentro, puntoEntradaEdificio, this);
        Debug.Log($"✅ Nuevo visitante: {datos.nombreVisitante}");
    }

    public VisitanteSimple ObtenerVisitanteActual() => visitanteActual;

    public void VisitanteTerminoSalir()
    {
        if (nocheTerminada) return;

        visitantesAtendidosEnNoche++;
        Debug.Log($"Visitante atendido. Total: {visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");

        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log("Máximo alcanzado");
            return;
        }

        if (esperandoVisitante) return;
        esperandoVisitante = true;

        EstadoVisitantes.Instancia.PasarAlSiguienteVisitante();
        EstadoVisitantes.Instancia.LimpiarEstadoGuardado();
        StartCoroutine(EsperarYCrearSiguiente());
    }

    IEnumerator EsperarYCrearSiguiente()
    {
        yield return new WaitForSeconds(tiempoEntreVisitantes);
        esperandoVisitante = false;

        if (!generacionVisitantesPausada && visitantesAtendidosEnNoche < maxVisitantesPorNoche && !nocheTerminada)
        {
            CrearVisitanteActual();
        }
        else if (generacionVisitantesPausada)
        {
            pendingVisitante = true;
            Debug.Log("📌 Pausado, pendiente para después");
        }
    }

    public void ReiniciarNoche()
    {
        if (visitanteActual != null) Destroy(visitanteActual.gameObject);
        esperandoVisitante = false;
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        pendingVisitante = false;
        EstadoVisitantes.Instancia?.LimpiarEstadoGuardado();
        if (!generacionVisitantesPausada && tutorialCompletado) CrearVisitanteActual();
    }

    public void ReiniciarJuegoCompleto()
    {
        if (EstadoVisitantes.Instancia != null) EstadoVisitantes.Instancia.ReiniciarEstado();
        tutorialCompletado = false;
        generacionVisitantesPausada = true;
        visitantesAtendidosEnNoche = 0;
        pendingVisitante = false;
        if (visitanteActual != null) Destroy(visitanteActual.gameObject);
    }

    public void TerminarNoche()
    {
        nocheTerminada = true;
        if (visitanteActual != null) Destroy(visitanteActual.gameObject);
    }

    public void PausarGeneracionVisitantes(bool pausar)
    {
        generacionVisitantesPausada = pausar;
        Debug.Log($"Generación visitantes {(pausar ? "pausada" : "reanudada")}");

        if (!pausar && pendingVisitante && !nocheTerminada && visitantesAtendidosEnNoche < maxVisitantesPorNoche)
        {
            Debug.Log("📌 Reanudando con visitante pendiente");
            pendingVisitante = false;
            StartCoroutine(CrearVisitanteConDelay());
        }
    }
}