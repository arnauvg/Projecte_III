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

    void Start()
    {
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;

        // Verificar si ya hay un visitante del estado guardado
        if (EstadoVisitantes.Instancia != null && EstadoVisitantes.Instancia.HayEstadoGuardado())
        {
            Debug.Log("Hay estado guardado, no crear nuevo visitante aún");
            CrearVisitanteActual();
        }
        else
        {
            CrearVisitanteActual();
        }
    }

    void CrearVisitanteActual()
    {
        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log($"Límite de {maxVisitantesPorNoche} visitantes alcanzado.");
            return;
        }

        if (EstadoVisitantes.Instancia == null)
        {
            Debug.LogError("No existe EstadoVisitantes en la escena.");
            return;
        }

        // Si el visitante ya fue atendido (estado guardado), avanzar al siguiente
        if (EstadoVisitantes.Instancia.VisitanteYaAtendido())
        {
            Debug.Log("Visitante ya fue atendido, pasando al siguiente");
            EstadoVisitantes.Instancia.PasarAlSiguienteVisitante();
            EstadoVisitantes.Instancia.LimpiarEstadoGuardado();
        }

        VisitanteDatos datos = EstadoVisitantes.Instancia.ObtenerVisitanteActual();
        if (datos == null)
        {
            Debug.Log("No quedan más visitantes en la lista.");
            return;
        }

        GameObject nuevoVisitante = Instantiate(prefabVisitante);
        visitanteActual = nuevoVisitante.GetComponent<VisitanteSimple>();
        if (visitanteActual == null)
        {
            Debug.LogError("El prefab no tiene el script VisitanteSimple.");
            return;
        }

        visitanteActual.ConfigurarVisitante(
            datos,
            puntoEntrada,
            puntoCentro,
            puntoEntradaEdificio,
            this
        );
    }

    public VisitanteSimple ObtenerVisitanteActual() => visitanteActual;

    public void VisitanteTerminoSalir()
    {
        if (nocheTerminada) return;

        visitantesAtendidosEnNoche++;
        Debug.Log($"Visitante atendido. Total en la noche: {visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");

        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log("Máximo de visitantes alcanzado. No se crearán más.");
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

        if (visitantesAtendidosEnNoche < maxVisitantesPorNoche && !nocheTerminada)
            CrearVisitanteActual();
    }

    public void ReiniciarNoche()
    {
        if (visitanteActual != null)
            Destroy(visitanteActual.gameObject);

        esperandoVisitante = false;
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;

        EstadoVisitantes.Instancia?.LimpiarEstadoGuardado();
        CrearVisitanteActual();
    }

    public void ReiniciarJuegoCompleto()
    {
        if (EstadoVisitantes.Instancia != null)
            EstadoVisitantes.Instancia.ReiniciarEstado();
        ReiniciarNoche();
    }

    public void TerminarNoche()
    {
        nocheTerminada = true;
        if (visitanteActual != null)
            Destroy(visitanteActual.gameObject);
    }
}