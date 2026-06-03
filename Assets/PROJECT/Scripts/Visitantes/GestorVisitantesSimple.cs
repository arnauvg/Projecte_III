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
    private bool generacionPausada = false;
    private bool creandoVisitante = false;

    void Start()
    {
        if (EstadoVisitantes.Instancia == null)
        {
            Debug.LogError("EstadoVisitantes no encontrado");
            return;
        }

        Debug.Log($"Gestor iniciado. Índice actual: {EstadoVisitantes.Instancia.indiceVisitanteActual}");

        if (!nocheTerminada && visitantesAtendidosEnNoche < maxVisitantesPorNoche)
        {
            CrearVisitanteActual();
        }
    }

    void CrearVisitanteActual()
    {
        if (creandoVisitante)
        {
            Debug.Log("Ya se está creando un visitante, ignorando");
            return;
        }

        if (generacionPausada)
        {
            Debug.Log("Generación pausada");
            return;
        }

        if (nocheTerminada)
        {
            Debug.Log("Noche terminada");
            return;
        }

        if (visitanteActual != null)
        {
            Debug.Log("Ya hay un visitante activo");
            return;
        }

        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log($"Máximo alcanzado: {visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");
            return;
        }

        VisitanteDatos datos = EstadoVisitantes.Instancia.ObtenerVisitanteActual();
        if (datos == null)
        {
            Debug.Log("No hay más visitantes");
            return;
        }

        creandoVisitante = true;
        Debug.Log($"Creando visitante {visitantesAtendidosEnNoche + 1}/{maxVisitantesPorNoche}: {datos.nombreVisitante} (índice {EstadoVisitantes.Instancia.indiceVisitanteActual})");

        GameObject nuevoVisitante = Instantiate(prefabVisitante);
        visitanteActual = nuevoVisitante.GetComponent<VisitanteSimple>();
        visitanteActual.ConfigurarVisitante(datos, puntoEntrada, puntoCentro, puntoEntradaEdificio, this);

        creandoVisitante = false;
    }

    public VisitanteSimple ObtenerVisitanteActual() => visitanteActual;

    public void VisitanteAtendido()
    {
        visitantesAtendidosEnNoche++;
        Debug.Log($"Visitante atendido {visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");
    }

    public void VisitanteTerminoSalir()
    {
        if (nocheTerminada) return;

        visitanteActual = null;

        // 🔥 Siempre avanzar al siguiente visitante cuando se va
        EstadoVisitantes.Instancia.SiguienteVisitante();

        if (visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log($"Máximo alcanzado ({visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}). No se crearán más visitantes esta noche.");
            return;
        }

        if (!esperandoVisitante && !creandoVisitante)
        {
            StartCoroutine(EsperarYCrearSiguiente());
        }
    }

    IEnumerator EsperarYCrearSiguiente()
    {
        if (esperandoVisitante) yield break;

        esperandoVisitante = true;
        yield return new WaitForSeconds(tiempoEntreVisitantes);
        esperandoVisitante = false;

        if (!generacionPausada && !nocheTerminada && visitanteActual == null &&
            visitantesAtendidosEnNoche < maxVisitantesPorNoche)
        {
            CrearVisitanteActual();
        }
    }

    public void ReiniciarNoche()
    {
        Debug.Log("=== REINICIANDO NOCHE ===");

        if (visitanteActual != null)
        {
            Destroy(visitanteActual.gameObject);
            visitanteActual = null;
        }

        esperandoVisitante = false;
        creandoVisitante = false;
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        generacionPausada = false;

        // 🔥 NO modificar el índice aquí. El índice ya avanzó al final de la noche anterior
        Debug.Log($"Reiniciando noche. Índice actual (NO MODIFICADO): {EstadoVisitantes.Instancia.indiceVisitanteActual}");

        CrearVisitanteActual();
    }

    public void ReiniciarJuegoCompleto()
    {
        Debug.Log("=== REINICIANDO JUEGO COMPLETO ===");

        if (visitanteActual != null)
        {
            Destroy(visitanteActual.gameObject);
            visitanteActual = null;
        }

        esperandoVisitante = false;
        creandoVisitante = false;
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        generacionPausada = false;

        if (EstadoVisitantes.Instancia != null)
        {
            EstadoVisitantes.Instancia.ReiniciarJuego();
        }

        CrearVisitanteActual();
    }

    public void TerminarNoche()
    {
        nocheTerminada = true;
        if (visitanteActual != null)
        {
            Destroy(visitanteActual.gameObject);
            visitanteActual = null;
        }
    }

    public void PausarGeneracionVisitantes(bool pausar)
    {
        generacionPausada = pausar;
        Debug.Log($"Generación visitantes {(pausar ? "pausada" : "reanudada")}");

        if (!pausar && visitanteActual == null && !nocheTerminada &&
            visitantesAtendidosEnNoche < maxVisitantesPorNoche && !creandoVisitante)
        {
            CrearVisitanteActual();
        }
    }
}