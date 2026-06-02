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
    private bool nocheTerminada = false;
    private bool visitanteActualYaRegistrado = false;

    private int visitantesAtendidosEnNoche = 0;

    void Start()
    {
        if (EstadoVisitantes.Instancia == null)
        {
            Debug.LogError("No existe EstadoVisitantes.");
            return;
        }

        visitantesAtendidosEnNoche = EstadoVisitantes.Instancia.visitantesAtendidosEstaNoche;
        nocheTerminada = EstadoVisitantes.Instancia.nocheTerminada;
        esperandoVisitante = false;

        Debug.Log($"Garita cargada. Visitantes atendidos: {visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");

        if (!nocheTerminada && visitantesAtendidosEnNoche < maxVisitantesPorNoche)
        {
            CrearVisitanteActual();
        }
    }

    void CrearVisitanteActual()
    {
        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log($"Límite de {maxVisitantesPorNoche} visitantes alcanzado.");
            visitanteActual = null;
            return;
        }

        if (EstadoVisitantes.Instancia == null)
        {
            Debug.LogError("No existe EstadoVisitantes en la escena.");
            return;
        }

        VisitanteDatos datos = EstadoVisitantes.Instancia.ObtenerVisitanteActual();

        if (datos == null)
        {
            Debug.Log("No quedan más visitantes en la lista.");
            visitanteActual = null;
            return;
        }

        GameObject nuevoVisitante = Instantiate(prefabVisitante);

        visitanteActual = nuevoVisitante.GetComponent<VisitanteSimple>();

        if (visitanteActual == null)
        {
            Debug.LogError("El prefab no tiene el script VisitanteSimple.");
            return;
        }

        visitanteActualYaRegistrado = false;

        visitanteActual.ConfigurarVisitante(
            datos,
            puntoEntrada,
            puntoCentro,
            puntoEntradaEdificio,
            this
        );
    }

    public VisitanteSimple ObtenerVisitanteActual()
    {
        return visitanteActual;
    }

    public void RegistrarVisitanteAtendido()
    {
        if (EstadoVisitantes.Instancia == null) return;
        if (nocheTerminada) return;
        if (visitanteActualYaRegistrado) return;

        visitanteActualYaRegistrado = true;

        EstadoVisitantes.Instancia.RegistrarVisitanteAtendido();

        visitantesAtendidosEnNoche = EstadoVisitantes.Instancia.visitantesAtendidosEstaNoche;

        Debug.Log($"Gestor actualizado. Visitantes atendidos: {visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");
    }

    public void VisitanteTerminoSalir()
    {
        if (nocheTerminada) return;

        if (EstadoVisitantes.Instancia == null) return;

        visitantesAtendidosEnNoche = EstadoVisitantes.Instancia.visitantesAtendidosEstaNoche;

        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log("Máximo de visitantes alcanzado. No se crearán más.");
            visitanteActual = null;
            return;
        }

        if (esperandoVisitante) return;

        esperandoVisitante = true;

        StartCoroutine(EsperarYCrearSiguiente());
    }

    IEnumerator EsperarYCrearSiguiente()
    {
        yield return new WaitForSeconds(tiempoEntreVisitantes);

        esperandoVisitante = false;

        if (visitantesAtendidosEnNoche < maxVisitantesPorNoche && !nocheTerminada)
        {
            CrearVisitanteActual();
        }
    }

    public void ReiniciarNoche()
    {
        if (visitanteActual != null)
        {
            Destroy(visitanteActual.gameObject);
        }

        esperandoVisitante = false;
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        visitanteActualYaRegistrado = false;

        if (EstadoVisitantes.Instancia != null)
        {
            EstadoVisitantes.Instancia.visitantesAtendidosEstaNoche = 0;
            EstadoVisitantes.Instancia.nocheTerminada = false;
        }

        CrearVisitanteActual();
    }

    public void ReiniciarJuegoCompleto()
    {
        if (EstadoVisitantes.Instancia != null)
        {
            EstadoVisitantes.Instancia.indiceVisitanteActual = 0;
            EstadoVisitantes.Instancia.visitantesAtendidosEstaNoche = 0;
            EstadoVisitantes.Instancia.nocheTerminada = false;
        }

        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        visitanteActualYaRegistrado = false;

        ReiniciarNoche();
    }

    public void TerminarNoche()
    {
        nocheTerminada = true;

        if (EstadoVisitantes.Instancia != null)
        {
            EstadoVisitantes.Instancia.nocheTerminada = true;
        }

        if (visitanteActual != null)
        {
            Destroy(visitanteActual.gameObject);
        }
    }
}