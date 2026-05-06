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

    [Header("UI - Pantalla")]
    public GameObject pantallaFinNoche;
    public TextMeshProUGUI textoNoche;
    public TextMeshProUGUI textoEstado;

    [Header("UI - Visitantes")]
    public TextMeshProUGUI textoNumVisitantes;   // "Num visitantes"
    public TextMeshProUGUI textoDineroVisitantes; // "Dinero visitantes"

    [Header("UI - Tareas")]
    public TextMeshProUGUI textoNumTareas;       // "Num tareas"
    public TextMeshProUGUI textoDineroTareas;    // "Dinero tareas"

    [Header("UI - Botón")]
    public Button botonContinuar;

    public GestorVisitantes gestorVisitantes;

    private int sueldoActual;
    private int nocheActual = 1;
    private bool visitanteCorrecto = false;
    private bool tareaCompletada = false;
    private int penalizacionVisitante = 0;

    void Start()
    {
        sueldoActual = sueldoBase;
        pantallaFinNoche.SetActive(false);
        Debug.Log($"NOCHE {nocheActual} - Sueldo: {sueldoActual}€");
    }

    public void RegistrarVisitanteAceptado()
    {
        penalizacionVisitante = penalizacionVisitanteIncorrecto;
        visitanteCorrecto = false;
        sueldoActual -= penalizacionVisitante;
        Debug.Log($"Visitante INCORRECTO! -{penalizacionVisitante}€");

        if (gestorVisitantes != null)
            gestorVisitantes.RegistrarRespuestaVisitante();
    }

    public void RegistrarVisitanteRechazado()
    {
        penalizacionVisitante = 0;
        visitanteCorrecto = true;
        Debug.Log($"Visitante CORRECTO! Sin penalización");

        if (gestorVisitantes != null)
            gestorVisitantes.RegistrarRespuestaVisitante();
    }

    public void CompletarTarea()
    {
        tareaCompletada = true;
        Debug.Log("Tarea completada!");
    }

    public void TerminarNoche()
    {
        Debug.Log("=== FIN DE LA NOCHE ===");

        int penalizacionTarea = tareaCompletada ? 0 : 100;
        if (penalizacionTarea > 0)
        {
            sueldoActual -= penalizacionTarea;
            Debug.Log($"Tarea pendiente! -{penalizacionTarea}€");
        }

        MostrarPantallaFinNoche(penalizacionTarea);
    }

    void MostrarPantallaFinNoche(int penalizacionTarea)
    {
        textoNoche.text = $"NOCHE {nocheActual}";

        // Visitantes
        if (textoNumVisitantes != null)
            textoNumVisitantes.text = visitanteCorrecto ? "1/1" : "0/1";
        if (textoDineroVisitantes != null)
            textoDineroVisitantes.text = $"-{penalizacionVisitante}€";

        // Tareas
        if (textoNumTareas != null)
            textoNumTareas.text = tareaCompletada ? "1/1" : "0/1";
        if (textoDineroTareas != null)
            textoDineroTareas.text = $"-{penalizacionTarea}€";

        // Colores
        if (textoDineroVisitantes != null)
            textoDineroVisitantes.color = penalizacionVisitante > 0 ? Color.red : Color.green;
        if (textoDineroTareas != null)
            textoDineroTareas.color = penalizacionTarea > 0 ? Color.red : Color.green;

        bool despedido = sueldoActual < umbralDespido;
        bool victoria = !despedido && nocheActual >= 5;

        botonContinuar.onClick.RemoveAllListeners();

        if (despedido)
        {
            textoEstado.text = "💀 GAME OVER 💀";
            botonContinuar.GetComponentInChildren<TextMeshProUGUI>().text = "REINICIAR";
            botonContinuar.onClick.AddListener(ReiniciarJuego);
        }
        else if (victoria)
        {
            textoEstado.text = "🏆 VICTORIA 🏆";
            botonContinuar.GetComponentInChildren<TextMeshProUGUI>().text = "JUGAR DE NUEVO";
            botonContinuar.onClick.AddListener(ReiniciarJuego);
        }
        else
        {
            textoEstado.text = "COMPLETADA";
            botonContinuar.GetComponentInChildren<TextMeshProUGUI>().text = "SIGUIENTE NOCHE";
            botonContinuar.onClick.AddListener(SiguienteNoche);
        }

        pantallaFinNoche.SetActive(true);
        Time.timeScale = 0f;
    }

    void SiguienteNoche()
    {
        nocheActual++;
        visitanteCorrecto = false;
        tareaCompletada = false;
        penalizacionVisitante = 0;

        pantallaFinNoche.SetActive(false);
        Time.timeScale = 1f;

        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
        if (visitante != null)
            visitante.ReiniciarParaNuevaNoche();

        Debug.Log($"NOCHE {nocheActual} - Sueldo: {sueldoActual}€");
    }

    void ReiniciarJuego()
    {
        nocheActual = 1;
        sueldoActual = sueldoBase;
        visitanteCorrecto = false;
        tareaCompletada = false;
        penalizacionVisitante = 0;

        pantallaFinNoche.SetActive(false);
        Time.timeScale = 1f;

        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
        if (visitante != null)
            visitante.ReiniciarParaNuevaNoche();

        Debug.Log("JUEGO REINICIADO");
    }
}