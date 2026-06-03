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
        indiceVisitanteActual++;
        Debug.Log($"👉 Avanzando índice. Nuevo índice: {indiceVisitanteActual}");
    }

    public void ReiniciarJuego()
    {
        indiceVisitanteActual = 0;
        Debug.Log("EstadoVisitantes: Juego reiniciado - índice=0");
    }
}