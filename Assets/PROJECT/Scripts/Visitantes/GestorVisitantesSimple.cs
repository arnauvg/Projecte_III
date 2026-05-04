using UnityEngine;

public class GestorVisitantes : MonoBehaviour
{
    [Header("Referencias")]
    public VisitanteSimple visitante;
    public GestionNoches gestionNoches;
    public float tiempoPrimeraAparicion = 1f;

    private int visitantesLlamadosEnNoche = 0;
    private bool juegoInicializado = false;

    void Start()
    {
        // Buscar visitante si no está asignado o fue destruido
        if (visitante == null || !visitante.EstaVivo())
        {
            visitante = FindFirstObjectByType<VisitanteSimple>();
            if (visitante == null)
            {
                Debug.LogError("No hay visitante asignado y no se encontró uno en la escena!");
                return;
            }
            Debug.Log("Visitante encontrado automáticamente");
        }

        // Buscar GestionNoches si no está asignado
        if (gestionNoches == null)
        {
            gestionNoches = FindFirstObjectByType<GestionNoches>();
            if (gestionNoches == null)
            {
                Debug.LogError("No se encontró GestionNoches en la escena!");
                return;
            }
        }

        // Solo llamar al primer visitante si es la primera vez
        if (!juegoInicializado)
        {
            juegoInicializado = true;
            Invoke(nameof(LlamarVisitante), tiempoPrimeraAparicion);
        }
    }

    void LlamarVisitante()
    {
        // Verificar que el visitante sigue existiendo
        if (visitante == null || !visitante.EstaVivo())
        {
            Debug.LogError("El visitante ha sido destruido! Buscando uno nuevo...");
            visitante = FindFirstObjectByType<VisitanteSimple>();
            if (visitante == null)
            {
                Debug.LogError("No se encontró visitante, no se puede continuar");
                return;
            }
        }

        if (visitantesLlamadosEnNoche >= 3)
        {
            Debug.Log("Ya se atendieron los 3 visitantes de esta noche");
            return;
        }

        if (gestionNoches != null && gestionNoches.EstaNocheActiva())
        {
            Debug.Log($"Llamando al visitante... ({visitantesLlamadosEnNoche + 1}/3)");
            visitante.ReiniciarParaNuevaNoche();
            visitante.Aparecer();
            visitantesLlamadosEnNoche++;
        }
        else
        {
            Debug.Log("No se puede llamar visitante: noche no activa o sistema no disponible");
        }
    }

    public void ReiniciarNoche()
    {
        visitantesLlamadosEnNoche = 0;
        CancelInvoke();
        Invoke(nameof(LlamarVisitante), tiempoPrimeraAparicion);
        Debug.Log("GestorVisitantes: Noche reiniciada");
    }

    public void ReiniciarJuego()
    {
        visitantesLlamadosEnNoche = 0;
        juegoInicializado = false;
        CancelInvoke();
        Debug.Log("GestorVisitantes: Juego reiniciado");
    }

    public int GetVisitantesAtendidos()
    {
        return visitantesLlamadosEnNoche;
    }
}