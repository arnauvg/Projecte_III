using UnityEngine;

public class PausaManager : MonoBehaviour
{
    public static PausaManager Instance { get; private set; }

    [Header("Estado actual")]
    public bool juegoPausado = false;
    public bool dialogoActivo = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("PausaManager: Inicializado");
    }

    public void PausarJuego()
    {
        if (juegoPausado) return;

        juegoPausado = true;
        dialogoActivo = true;

        Debug.Log("⏸ Juego pausado");

        // Buscar componentes en escena actual
        CronometroNoche crono = FindFirstObjectByType<CronometroNoche>();
        if (crono != null) crono.DetenerCronometro();

        TareaManager tarea = FindFirstObjectByType<TareaManager>();
        if (tarea != null) tarea.PausarGeneracionTareas(true);

        GestorVisitantesSimple gestor = FindFirstObjectByType<GestorVisitantesSimple>();
        if (gestor != null) gestor.PausarGeneracionVisitantes(true);
    }

    public void ReanudarJuego()
    {
        if (!juegoPausado) return;

        juegoPausado = false;
        dialogoActivo = false;

        Debug.Log("▶ Juego reanudado");

        CronometroNoche crono = FindFirstObjectByType<CronometroNoche>();
        if (crono != null) crono.ReanudarCronometro();

        TareaManager tarea = FindFirstObjectByType<TareaManager>();
        if (tarea != null) tarea.PausarGeneracionTareas(false);

        GestorVisitantesSimple gestor = FindFirstObjectByType<GestorVisitantesSimple>();
        if (gestor != null) gestor.PausarGeneracionVisitantes(false);
    }
}