using UnityEngine;
using System.Collections.Generic;

public class EstadoVisitantes : MonoBehaviour
{
    public static EstadoVisitantes Instancia;

    [Header("Visitantes en orden (global)")]
    public List<VisitanteDatos> visitantes;

    [Header("Estado actual")]
    public int indiceVisitanteActual = 0;

    [Header("Estado de la noche")]
    public int visitantesAtendidosEnNoche = 0;
    public bool nocheTerminada = false;
    public bool visitanteActivo = false;
    public string nombreVisitanteActivo = "";

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        Debug.Log("EstadoVisitantes: Inicializado");
    }

    public VisitanteDatos ObtenerVisitanteActual()
    {
        if (visitantes == null || visitantes.Count == 0)
        {
            Debug.LogError("No hay visitantes asignados.");
            return null;
        }
        if (indiceVisitanteActual >= visitantes.Count)
        {
            Debug.Log($"No hay más visitantes. Índice: {indiceVisitanteActual}, Total: {visitantes.Count}");
            return null;
        }
        return visitantes[indiceVisitanteActual];
    }

    public void SiguienteVisitante()
    {
        int indiceAnterior = indiceVisitanteActual;
        indiceVisitanteActual++;
        Debug.Log($"👉 Avanzando índice: {indiceAnterior} → {indiceVisitanteActual}");
    }

    public void RegistrarVisitanteAtendido()
    {
        visitantesAtendidosEnNoche++;
        Debug.Log($"Visitante atendido esta noche: {visitantesAtendidosEnNoche}");
    }

    public void ReiniciarNoche()
    {
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        visitanteActivo = false;
        Debug.Log($"EstadoVisitantes: NOCHE REINICIADA - atendidos=0, índice ACTUAL={indiceVisitanteActual}");
    }

    public void ReiniciarJuego()
    {
        indiceVisitanteActual = 0;
        visitantesAtendidosEnNoche = 0;
        nocheTerminada = false;
        visitanteActivo = false;
        Debug.Log("EstadoVisitantes: JUEGO REINICIADO - índice=0");
    }
}