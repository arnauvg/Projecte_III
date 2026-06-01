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
        CrearVisitanteActual();
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

        StartCoroutine(EsperarYCrearSiguiente());
    }

    IEnumerator EsperarYCrearSiguiente()
    {
        yield return new WaitForSeconds(tiempoEntreVisitantes);
        esperandoVisitante = false;

        if (visitantesAtendidosEnNoche < maxVisitantesPorNoche && !nocheTerminada)
            CrearVisitanteActual();
    }

    // Reinicia la noche SIN reiniciar el índice global de visitantes
    public void ReiniciarNoche()
    {
        if (visitanteActual != null)
            Destroy(visitanteActual.gameObject);

        esperandoVisitante = false;
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;

        // 🔥 NO resetear el índice de visitantes (se mantiene el orden entre noches)
        CrearVisitanteActual();
    }

    // Reinicio completo del juego (sí resetea el índice)
    public void ReiniciarJuegoCompleto()
    {
        if (EstadoVisitantes.Instancia != null)
            EstadoVisitantes.Instancia.indiceVisitanteActual = 0;
        ReiniciarNoche();
    }

    public void TerminarNoche()
    {
        nocheTerminada = true;
        if (visitanteActual != null)
            Destroy(visitanteActual.gameObject);
    }
}