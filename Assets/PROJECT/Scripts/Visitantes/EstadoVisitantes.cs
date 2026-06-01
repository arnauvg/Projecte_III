using UnityEngine;
using System.Collections.Generic;

public class EstadoVisitantes : MonoBehaviour
{
    public static EstadoVisitantes Instancia;

    [Header("Visitantes en orden (global)")]
    public List<VisitanteDatos> visitantes;

    [Header("Estado actual")]
    public int indiceVisitanteActual = 0;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        if (transform.parent == null)
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
}