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
        Debug.Log($"Visitantes atendidos esta noche: {EstadoVisitantes.Instancia.visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");

        if (!EstadoVisitantes.Instancia.nocheTerminada &&
            EstadoVisitantes.Instancia.visitantesAtendidosEnNoche < maxVisitantesPorNoche)
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

        if (EstadoVisitantes.Instancia.nocheTerminada)
        {
            Debug.Log("Noche terminada");
            return;
        }

        if (visitanteActual != null)
        {
            Debug.Log("Ya hay un visitante activo");
            return;
        }

        if (EstadoVisitantes.Instancia.visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log($"Máximo alcanzado: {EstadoVisitantes.Instancia.visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");
            return;
        }

        VisitanteDatos datos = EstadoVisitantes.Instancia.ObtenerVisitanteActual();
        if (datos == null)
        {
            Debug.Log("No hay más visitantes");
            return;
        }

        creandoVisitante = true;
        EstadoVisitantes.Instancia.visitanteActivo = true;
        EstadoVisitantes.Instancia.nombreVisitanteActivo = datos.nombreVisitante;

        Debug.Log($"Creando visitante {EstadoVisitantes.Instancia.visitantesAtendidosEnNoche + 1}/{maxVisitantesPorNoche}: {datos.nombreVisitante} (índice {EstadoVisitantes.Instancia.indiceVisitanteActual})");

        GameObject nuevoVisitante = Instantiate(prefabVisitante);
        visitanteActual = nuevoVisitante.GetComponent<VisitanteSimple>();
        visitanteActual.ConfigurarVisitante(datos, puntoEntrada, puntoCentro, puntoEntradaEdificio, this);

        creandoVisitante = false;
    }

    public VisitanteSimple ObtenerVisitanteActual() => visitanteActual;

    public void VisitanteAtendido()
    {
        EstadoVisitantes.Instancia.RegistrarVisitanteAtendido();
        Debug.Log($"Visitante atendido {EstadoVisitantes.Instancia.visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}");
    }

    public void VisitanteTerminoSalir()
    {
        if (EstadoVisitantes.Instancia.nocheTerminada) return;

        visitanteActual = null;
        EstadoVisitantes.Instancia.visitanteActivo = false;

        // 🔥 PRIMERO avanzar el índice, DESPUÉS verificar límite
        EstadoVisitantes.Instancia.SiguienteVisitante();

        if (EstadoVisitantes.Instancia.visitantesAtendidosEnNoche >= maxVisitantesPorNoche)
        {
            Debug.Log($"Máximo alcanzado ({EstadoVisitantes.Instancia.visitantesAtendidosEnNoche}/{maxVisitantesPorNoche}). No se crearán más visitantes esta noche.");
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

        if (!generacionPausada && !EstadoVisitantes.Instancia.nocheTerminada &&
            visitanteActual == null &&
            EstadoVisitantes.Instancia.visitantesAtendidosEnNoche < maxVisitantesPorNoche)
        {
            CrearVisitanteActual();
        }
    }

    public void ReiniciarNoche()
    {
        Debug.Log("=== GESTOR: REINICIANDO NOCHE ===");

        if (visitanteActual != null)
        {
            Destroy(visitanteActual.gameObject);
            visitanteActual = null;
        }

        esperandoVisitante = false;
        creandoVisitante = false;
        generacionPausada = false;

        EstadoVisitantes.Instancia.ReiniciarNoche();

        Debug.Log($"Reiniciando noche. Índice ACTUAL: {EstadoVisitantes.Instancia.indiceVisitanteActual}");

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
        generacionPausada = false;

        if (EstadoVisitantes.Instancia != null)
        {
            EstadoVisitantes.Instancia.ReiniciarJuego();
        }

        CrearVisitanteActual();
    }

    public void TerminarNoche()
    {
        EstadoVisitantes.Instancia.nocheTerminada = true;
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

        if (!pausar && visitanteActual == null && !EstadoVisitantes.Instancia.nocheTerminada &&
            EstadoVisitantes.Instancia.visitantesAtendidosEnNoche < maxVisitantesPorNoche && !creandoVisitante)
        {
            CrearVisitanteActual();
        }
    }
}