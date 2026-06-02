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
    private bool generacionVisitantesPausada = true; // Comienza pausado
    private bool tutorialCompletado = false;

    void Start()
    {
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        generacionVisitantesPausada = true;
        tutorialCompletado = false;

        // Suscribirse al evento de fin de diálogo
        DialogueManager.OnDialogueEnded += TutorialCompletado;

        Debug.Log("GestorVisitantesSimple: Esperando fin del tutorial...");
    }

    void OnDestroy()
    {
        // Limpiar suscripción
        DialogueManager.OnDialogueEnded -= TutorialCompletado;
    }

    void TutorialCompletado()
    {
        if (tutorialCompletado) return;

        tutorialCompletado = true;
        generacionVisitantesPausada = false;
        Debug.Log("📞 Tutorial completado - Comienzan a llegar visitantes");

        CrearVisitanteActual();
    }

    void CrearVisitanteActual()
    {
        if (generacionVisitantesPausada) return;
        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche) return;
        if (EstadoVisitantes.Instancia == null) return;

        if (EstadoVisitantes.Instancia.VisitanteYaAtendido())
        {
            EstadoVisitantes.Instancia.PasarAlSiguienteVisitante();
            EstadoVisitantes.Instancia.LimpiarEstadoGuardado();
        }

        VisitanteDatos datos = EstadoVisitantes.Instancia.ObtenerVisitanteActual();
        if (datos == null) return;

        GameObject nuevoVisitante = Instantiate(prefabVisitante);
        visitanteActual = nuevoVisitante.GetComponent<VisitanteSimple>();
        visitanteActual.ConfigurarVisitante(datos, puntoEntrada, puntoCentro, puntoEntradaEdificio, this);
    }

    public VisitanteSimple ObtenerVisitanteActual() => visitanteActual;

    public void VisitanteTerminoSalir()
    {
        if (nocheTerminada) return;
        visitantesAtendidosEnNoche++;

        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche) return;
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
            CrearVisitanteActual();
    }

    public void ReiniciarNoche()
    {
        if (visitanteActual != null) Destroy(visitanteActual.gameObject);
        esperandoVisitante = false;
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        EstadoVisitantes.Instancia?.LimpiarEstadoGuardado();
        if (!generacionVisitantesPausada && tutorialCompletado) CrearVisitanteActual();
    }

    public void ReiniciarJuegoCompleto()
    {
        if (EstadoVisitantes.Instancia != null) EstadoVisitantes.Instancia.ReiniciarEstado();
        tutorialCompletado = false;
        generacionVisitantesPausada = true;
        visitantesAtendidosEnNoche = 0;
        if (visitanteActual != null) Destroy(visitanteActual.gameObject);
        Debug.Log("GestorVisitantesSimple: Reiniciado, esperando tutorial...");
    }

    public void TerminarNoche()
    {
        nocheTerminada = true;
        if (visitanteActual != null) Destroy(visitanteActual.gameObject);
    }

    public void PausarGeneracionVisitantes(bool pausar) => generacionVisitantesPausada = pausar;
}