using UnityEngine;
using TMPro;

public class CronometroNoche : MonoBehaviour
{
    [Header("Tiempo real")]
    public float tiempoTotalSegundos = 120f;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoReloj;

    [Header("Referencias del sistema de noches")]
    public GestionNoches gestionNoches;

    [Header("Sistema de tareas")]
    public TareaManager tareaManager;

    private float tiempoRestante;
    private bool nocheActiva = true;
    private int ultimoIntervaloMostrado = -1;
    private bool nocheTerminada = false;
    private bool tareasSpawned = false;

    void Start()
    {
        tiempoRestante = tiempoTotalSegundos;
        ActualizarTexto();

        if (gestionNoches == null)
            gestionNoches = FindFirstObjectByType<GestionNoches>();

        if (tareaManager == null)
            tareaManager = FindFirstObjectByType<TareaManager>();

        Debug.Log("⏰ Cronómetro iniciado");
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

        if (!tareasSpawned && ultimoIntervaloMostrado >= 1)
        {
            SpawnearTareas();
        }
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
            // ✅ AHORA SÍ FUNCIONA
            int noche = gestionNoches != null ? gestionNoches.GetNocheActual() : 1;
            tareaManager.IniciarNoche(noche);
        }
    }

    void TerminarNoche()
    {
        if (nocheTerminada) return;

        nocheTerminada = true;
        nocheActiva = false;

        Debug.Log("🌙 LA NOCHE HA TERMINADO - 06:00 AM");
        textoReloj.text = "06:00 AM";

        if (gestionNoches != null)
        {
            gestionNoches.TerminarNochePorTiempo();
        }
    }

    public void ReiniciarCronometro()
    {
        tiempoRestante = tiempoTotalSegundos;
        nocheActiva = true;
        nocheTerminada = false;
        tareasSpawned = false;
        ultimoIntervaloMostrado = -1;
        ActualizarTexto();
        Debug.Log("🔄 Cronómetro reiniciado");
    }

    public void DetenerCronometro()
    {
        nocheActiva = false;
    }

    public void ReanudarCronometro()
    {
        if (!nocheTerminada)
            nocheActiva = true;
    }
}