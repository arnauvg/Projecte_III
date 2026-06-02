using UnityEngine;
using System.Collections.Generic;

public class EstadoVisitantes : MonoBehaviour
{
    public static EstadoVisitantes Instancia;

    [Header("Visitantes en orden (global)")]
    public List<VisitanteDatos> visitantes;

    [Header("Estado actual")]
    public int indiceVisitanteActual = 0;

    [Header("Estado persistente del visitante actual")]
    public bool visitanteEnCentro = false;
    public bool visitanteYaAtendido = false;
    public string visitanteNombre = "";
    public int visitanteIndice = -1;

    [Header("Posición del visitante (para restaurar)")]
    public Vector3 visitantePosicion;

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
        // Limpiar estado guardado al pasar al siguiente
        LimpiarEstadoGuardado();
        Debug.Log($"Siguiente visitante índice: {indiceVisitanteActual}");
    }

    public void GuardarEstadoVisitante(VisitanteSimple visitante)
    {
        if (visitante != null && visitante.datosVisitante != null)
        {
            visitanteEnCentro = visitante.enCentro;
            visitanteYaAtendido = visitante.yaAtendido;
            visitanteNombre = visitante.datosVisitante.nombreVisitante;
            visitanteIndice = indiceVisitanteActual;
            visitantePosicion = visitante.transform.position;
            Debug.Log($"✅ Estado guardado: {visitanteNombre}, enCentro={visitanteEnCentro}, atendido={visitanteYaAtendido}");
        }
    }

    public bool HayEstadoGuardado()
    {
        return visitanteIndice == indiceVisitanteActual && !visitanteYaAtendido;
    }

    public bool VisitanteYaAtendido()
    {
        return visitanteIndice == indiceVisitanteActual && visitanteYaAtendido;
    }

    public void LimpiarEstadoGuardado()
    {
        visitanteEnCentro = false;
        visitanteYaAtendido = false;
        visitanteNombre = "";
        visitanteIndice = -1;
        visitantePosicion = Vector3.zero;
    }

    public void ReiniciarEstado()
    {
        indiceVisitanteActual = 0;
        LimpiarEstadoGuardado();
    }

    public void DebugVisitantes()
    {
        Debug.Log($"Visitante actual índice: {indiceVisitanteActual}, total: {visitantes.Count}");
        if (indiceVisitanteActual < visitantes.Count)
            Debug.Log($"Siguiente visitante: {visitantes[indiceVisitanteActual].nombreVisitante}");
        else
            Debug.Log("No hay más visitantes");
    }
}