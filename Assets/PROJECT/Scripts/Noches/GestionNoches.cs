using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GestionNoches : MonoBehaviour
{
    [Header("Sueldo")]
    public int sueldoBase = 1500;
    public int umbralDespido = 300;

    [Header("Penalizaciones")]
    public int penalizacionVisitanteIncorrecto = 50;
    public int penalizacionTareaFallada = 100;

    [Header("UI")]
    public GameObject pantallaFinNoche;
    public TextMeshProUGUI textoNoche;
    public TextMeshProUGUI textoEstado;
    public TextMeshProUGUI textoVisitantes;
    public TextMeshProUGUI textoDineroVisitantes;
    public TextMeshProUGUI textoTareas;
    public TextMeshProUGUI textoDineroTareas;
    public Button botonContinuar;

    [Header("Referencias")]
    public GestorVisitantes gestorVisitantes;

    // Variables internas
    private int sueldoActual;
    private int nocheActual = 1;
    private int visitantesCorrectos = 0;
    private int visitantesIncorrectos = 0;
    private int tareasCompletadas = 0;
    private bool nocheTerminada = false;
    private bool juegoTerminado = false;

    void Start()
    {
        sueldoActual = sueldoBase;
        pantallaFinNoche.SetActive(false);

        Debug.Log($"=== NOCHE {nocheActual} ===");
        Debug.Log($"Sueldo: {sueldoActual}€ | Despido por debajo de {umbralDespido}€");
    }

    // Llamado por VisitanteSimple cuando usan el botón VERDE (INCORRECTO)
    public void RegistrarVisitanteAceptado()
    {
        if (nocheTerminada) return;

        visitantesIncorrectos++;
        sueldoActual -= penalizacionVisitanteIncorrecto;

        Debug.Log($"👎 Visitante INCORRECTO! -{penalizacionVisitanteIncorrecto}€. Sueldo: {sueldoActual}€");

        // Notificar al gestor
        if (gestorVisitantes != null)
            gestorVisitantes.RegistrarRespuestaVisitante();
    }

    // Llamado por VisitanteSimple cuando usan el botón ROJO (CORRECTO)
    public void RegistrarVisitanteRechazado()
    {
        if (nocheTerminada) return;

        visitantesCorrectos++;
        Debug.Log($"👍 Visitante CORRECTO! Sueldo: {sueldoActual}€");

        // Notificar al gestor
        if (gestorVisitantes != null)
            gestorVisitantes.RegistrarRespuestaVisitante();
    }

    // Llamado por el minijuego de velas al completarlo
    public void CompletarTarea()
    {
        if (nocheTerminada) return;

        tareasCompletadas++;
        Debug.Log($"📋 Tarea completada ({tareasCompletadas}/2)");
    }

    // Llamado por GestorVisitantes cuando se atiende al último visitante
    public void TerminarNoche()
    {
        if (nocheTerminada) return;

        nocheTerminada = true;

        // Calcular penalización por tareas pendientes
        int tareasPendientes = 2 - tareasCompletadas;
        int penalizacionTareas = tareasPendientes * penalizacionTareaFallada;

        if (penalizacionTareas > 0)
        {
            sueldoActual -= penalizacionTareas;
            Debug.Log($"⚠️ Tareas pendientes: {tareasPendientes}. Penalización: -{penalizacionTareas}€");
        }

        // Mostrar resultados
        MostrarPantallaFinNoche(penalizacionTareas);
    }

    void MostrarPantallaFinNoche(int penalizacionTareas)
    {
        int penalizacionVisitantes = visitantesIncorrectos * penalizacionVisitanteIncorrecto;

        // Actualizar UI
        textoNoche.text = $"NOCHE {nocheActual}";

        // Visitantes
        textoVisitantes.text = $"{visitantesCorrectos}/3";
        textoDineroVisitantes.text = $"-{penalizacionVisitantes}€";
        textoDineroVisitantes.color = penalizacionVisitantes > 0 ? Color.red : Color.green;

        // Tareas
        textoTareas.text = $"{tareasCompletadas}/2";
        textoDineroTareas.text = $"-{penalizacionTareas}€";
        textoDineroTareas.color = penalizacionTareas > 0 ? Color.red : Color.green;

        // Verificar fin del juego
        bool despedido = sueldoActual < umbralDespido;
        bool victoria = !despedido && nocheActual >= 5;

        if (despedido)
        {
            textoEstado.text = "💀 GAME OVER 💀";
            textoEstado.color = Color.red;
            botonContinuar.GetComponentInChildren<TextMeshProUGUI>().text = "REINICIAR";
            juegoTerminado = true;
        }
        else if (victoria)
        {
            textoEstado.text = "🏆 ¡VICTORIA! 🏆";
            textoEstado.color = Color.yellow;
            botonContinuar.GetComponentInChildren<TextMeshProUGUI>().text = "JUGAR DE NUEVO";
            juegoTerminado = true;
        }
        else
        {
            textoEstado.text = "COMPLETADA";
            textoEstado.color = Color.green;
            botonContinuar.GetComponentInChildren<TextMeshProUGUI>().text = "SIGUIENTE NOCHE";
        }

        // Configurar botón
        botonContinuar.onClick.RemoveAllListeners();

        if (despedido || victoria)
            botonContinuar.onClick.AddListener(ReiniciarJuego);
        else
            botonContinuar.onClick.AddListener(SiguienteNoche);

        // Mostrar pantalla y pausar
        pantallaFinNoche.SetActive(true);
        Time.timeScale = 0f;
    }

    void SiguienteNoche()
    {
        nocheActual++;
        visitantesCorrectos = 0;
        visitantesIncorrectos = 0;
        tareasCompletadas = 0;
        nocheTerminada = false;

        pantallaFinNoche.SetActive(false);
        Time.timeScale = 1f;

        Debug.Log($"\n=== NOCHE {nocheActual} ===");
        Debug.Log($"Sueldo: {sueldoActual}€");

        if (gestorVisitantes != null)
            gestorVisitantes.IniciarNoche();
    }

    void ReiniciarJuego()
    {
        nocheActual = 1;
        sueldoActual = sueldoBase;
        visitantesCorrectos = 0;
        visitantesIncorrectos = 0;
        tareasCompletadas = 0;
        nocheTerminada = false;
        juegoTerminado = false;

        pantallaFinNoche.SetActive(false);
        Time.timeScale = 1f;

        Debug.Log("=== JUEGO REINICIADO ===");

        if (gestorVisitantes != null)
            gestorVisitantes.IniciarNoche();
    }
}