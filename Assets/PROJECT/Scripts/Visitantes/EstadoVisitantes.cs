using UnityEngine;

public class EstadoVisitantes : MonoBehaviour
{
    public static EstadoVisitantes Instancia;

    [Header("Lista de visitantes en orden")]
    public VisitanteDatos[] visitantes;

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
        DontDestroyOnLoad(gameObject);
    }

    public VisitanteDatos ObtenerVisitanteActual()
    {
        if (visitantes == null || visitantes.Length == 0)
        {
            Debug.LogError("No hay visitantes asignados en EstadoVisitantes.");
            return null;
        }

        if (indiceVisitanteActual >= visitantes.Length)
        {
            Debug.Log("Ya no quedan más visitantes.");
            return null;
        }

        return visitantes[indiceVisitanteActual];
    }

    public void PasarAlSiguienteVisitante()
    {
        indiceVisitanteActual++;

        if (indiceVisitanteActual >= visitantes.Length)
        {
            Debug.Log("Se han acabado todos los visitantes.");
        }
        else
        {
            Debug.Log("Siguiente visitante: " + indiceVisitanteActual);
        }
    }
}