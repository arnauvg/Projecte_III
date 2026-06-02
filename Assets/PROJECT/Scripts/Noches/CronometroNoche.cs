using UnityEngine;
using TMPro;

public class CronometroNoche : MonoBehaviour
{
    public static CronometroNoche Instance { get; private set; }

    [Header("Tiempo real")]
    public float tiempoTotalSegundos = 120f;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoReloj;

    [Header("Referencias del sistema de noches")]
    public GestionNoches gestionNoches;

    [Header("Sistema de tareas")]
    public TareaManager tareaManager;

    // 🔒 Datos estáticos para que persistan aunque se destruya el objeto
    private static float tiempoRestanteStatic;
    private static bool nocheActivaStatic = true;
    private static int ultimoIntervaloMostradoStatic = -1;
    private static bool nocheTerminadaStatic = false;
    private static bool tareasSpawnedStatic = false;
    private static bool inicializadoStatic = false;

    private float tiempoRestante;
    private bool nocheActiva;
    private int ultimoIntervaloMostrado;
    private bool nocheTerminada;
    private bool tareasSpawned;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (!inicializadoStatic)
        {
            // Primera vez: valores iniciales
            tiempoRestanteStatic = tiempoTotalSegundos;
            nocheActivaStatic = true;
            ultimoIntervaloMostradoStatic = -1;
            nocheTerminadaStatic = false;
            tareasSpawnedStatic = false;
            inicializadoStatic = true;
            Debug.Log("⏰ Cronómetro: primera inicialización");
        }

        // Cargar desde estáticos
        tiempoRestante = tiempoRestanteStatic;
        nocheActiva = nocheActivaStatic;
        ultimoIntervaloMostrado = ultimoIntervaloMostradoStatic;
        nocheTerminada = nocheTerminadaStatic;
        tareasSpawned = tareasSpawnedStatic;

        ActualizarTexto();

        if (gestionNoches == null)
            gestionNoches = FindFirstObjectByType<GestionNoches>();
        if (tareaManager == null)
            tareaManager = FindFirstObjectByType<TareaManager>();
    }

    void Update()
    {
        if (!nocheActiva) return;
        if (nocheTerminada) return;
        tiempoRestante -= Time.unscaledDeltaTime;
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            TerminarNoche();
        }

        ActualizarTexto();

        //if (!tareasSpawned && ultimoIntervaloMostrado >= 1)
        //{
        //    SpawnearTareas();
        //}

        // Guardar en estáticos cada frame
        tiempoRestanteStatic = tiempoRestante;
        nocheActivaStatic = nocheActiva;
        ultimoIntervaloMostradoStatic = ultimoIntervaloMostrado;
        nocheTerminadaStatic = nocheTerminada;
        tareasSpawnedStatic = tareasSpawned;
    }

    void ActualizarTexto()
    {
        float progreso = 1f - (tiempoRestante / tiempoTotalSegundos);
        float horasFloat = progreso * 6f;
        int horasEnteras = Mathf.FloorToInt(horasFloat);
        int minutos = Mathf.FloorToInt((horasFloat - horasEnteras) * 60);
        int minutosRedondeados = minutos < 30 ? 0 : 30;
        string ampm = "AM";
        string horaFormateada = $"{horasEnteras:00}:{minutosRedondeados:00} {ampm}";
        int intervaloActual = horasEnteras * 2 + (minutosRedondeados / 30);

        if (intervaloActual != ultimoIntervaloMostrado)
        {
            ultimoIntervaloMostrado = intervaloActual;
            textoReloj.text = horaFormateada;
        }
    }

    void SpawnearTareas()
    {
        if (tareasSpawned) return;
        tareasSpawned = true;
        Debug.Log("📋 Generando tareas para esta noche...");
        if (tareaManager != null)
        {
            int noche = gestionNoches != null ? gestionNoches.GetNocheActual() : 1;
            tareaManager.IniciarNoche(noche);
        }
    }

    void TerminarNoche()
    {
        if (nocheTerminada) return;
        nocheTerminada = true;
        nocheActiva = false;
        Debug.Log("🌙 NOCHE TERMINADA - 06:00 AM");
        textoReloj.text = "06:00 AM";
        if (gestionNoches != null)
            gestionNoches.TerminarNochePorTiempo();
    }

    public void ReiniciarCronometro()
    {
        tiempoRestanteStatic = tiempoTotalSegundos;
        nocheActivaStatic = true;
        nocheTerminadaStatic = false;
        tareasSpawnedStatic = false;
        ultimoIntervaloMostradoStatic = -1;
        // Recargar en instancia
        tiempoRestante = tiempoRestanteStatic;
        nocheActiva = nocheActivaStatic;
        nocheTerminada = nocheTerminadaStatic;
        tareasSpawned = tareasSpawnedStatic;
        ultimoIntervaloMostrado = ultimoIntervaloMostradoStatic;
        ActualizarTexto();
        Debug.Log("🔄 Cronómetro reiniciado (nueva noche)");
    }

    public void DetenerCronometro() => nocheActiva = false;
    public void ReanudarCronometro() { if (!nocheTerminada) nocheActiva = true; }
}