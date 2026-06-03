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

    public static System.Action<int, int> OnHoraCambiada;

    private float tiempoRestante;
    private bool nocheActiva = true;
    private int ultimoIntervaloMostrado = -1;
    private bool nocheTerminada = false;
    private bool tareasSpawned = false;

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
        tiempoRestante = tiempoTotalSegundos;
        if (textoReloj == null)
            textoReloj = FindFirstObjectByType<TextMeshProUGUI>();
        ActualizarTexto();
        Debug.Log("Cronómetro iniciado");
    }

    void Update()
    {
        if (!nocheActiva) return;
        if (nocheTerminada) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            TerminarNoche();
        }

        ActualizarTexto();

        bool juegoPausado = PausaManager.Instance != null && PausaManager.Instance.juegoPausado;
        if (!tareasSpawned && !juegoPausado && ultimoIntervaloMostrado >= 1)
        {
            tareasSpawned = true;
            Debug.Log("📋 Iniciando sistema de tareas...");
            if (tareaManager != null)
                tareaManager.IniciarNoche(1);
        }
    }

    void ActualizarTexto()
    {
        if (textoReloj == null) return;

        float progreso = 1f - (tiempoRestante / tiempoTotalSegundos);
        float horasFloat = progreso * 6f;
        int horasEnteras = Mathf.FloorToInt(horasFloat);
        int minutos = Mathf.FloorToInt((horasFloat - horasEnteras) * 60);
        int minutosRedondeados = minutos < 30 ? 0 : 30;
        string horaFormateada = $"{horasEnteras:00}:{minutosRedondeados:00} AM";
        int intervaloActual = horasEnteras * 2 + (minutosRedondeados / 30);

        if (intervaloActual != ultimoIntervaloMostrado)
        {
            ultimoIntervaloMostrado = intervaloActual;
            textoReloj.text = horaFormateada;

            // 🔥 Solo disparar evento en HORAS EN PUNTO (minutos = 0) entre 01:00 y 05:00
            if (minutosRedondeados == 0 && horasEnteras >= 1 && horasEnteras <= 5)
            {
                OnHoraCambiada?.Invoke(horasEnteras, minutosRedondeados);
                Debug.Log($"🕐 Evento hora en punto: {horasEnteras}:{minutosRedondeados:00}");
            }
        }
    }

    void TerminarNoche()
    {
        if (nocheTerminada) return;
        nocheTerminada = true;
        nocheActiva = false;
        Debug.Log("🌙 NOCHE TERMINADA - 06:00 AM");
        if (textoReloj != null) textoReloj.text = "06:00 AM";

        if (gestionNoches != null)
            gestionNoches.TerminarNochePorTiempo();
    }

    public void ReiniciarCronometro()
    {
        tiempoRestante = tiempoTotalSegundos;
        nocheActiva = true;
        nocheTerminada = false;
        tareasSpawned = false;
        ultimoIntervaloMostrado = -1;
        ActualizarTexto();
        Debug.Log("🔄 Cronómetro reiniciado (nueva noche)");
    }

    public void DetenerCronometro() => nocheActiva = false;
    public void ReanudarCronometro() { if (!nocheTerminada) nocheActiva = true; }
}