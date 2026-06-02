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
    public int visitantesAtendidosEstaNoche = 0;
    public bool nocheTerminada = false;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        // Lo sacamos del padre para que Unity permita DontDestroyOnLoad
        transform.SetParent(null);

        DontDestroyOnLoad(gameObject);
    }

    public VisitanteDatos ObtenerVisitanteActual()
    {
        if (visitantes == null || visitantes.Count == 0)
        {
            Debug.LogError("No hay visitantes asignados.");
            return null;
        }
        if (indiceVisitanteActual >= visitantes.Count)
            return null;
        return visitantes[indiceVisitanteActual];
    }

    public void PasarAlSiguienteVisitante()
    {
        indiceVisitanteActual++;
        Debug.Log($"Siguiente visitante índice: {indiceVisitanteActual}");
    }

    public void RegistrarVisitanteAtendido()
    {
        visitantesAtendidosEstaNoche++;
        indiceVisitanteActual++;

        Debug.Log($"Visitante guardado en EstadoVisitantes. Atendidos: {visitantesAtendidosEstaNoche}, índice actual: {indiceVisitanteActual}");
    }
    public void ReiniciarVisitantes()
    {
        indiceVisitanteActual = 0;
    }
}