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

    [Header("UI")]
    public GameObject pantallaFinNoche;
    public TextMeshProUGUI textoNoche;
    public TextMeshProUGUI textoEstado;
    public TextMeshProUGUI textoVisitantes;
    public TextMeshProUGUI textoDineroVisitantes;
    public TextMeshProUGUI textoTareas;
    public TextMeshProUGUI textoDineroTareas;
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
        // Aceptar = INCORRECTO (dejar pasar al vampiro)
        penalizacionVisitante = penalizacionVisitanteIncorrecto;
        visitanteCorrecto = false;
        sueldoActual -= penalizacionVisitante;
        Debug.Log($"Visitante INCORRECTO! -{penalizacionVisitante}€");

        if (gestorVisitantes != null)
            gestorVisitantes.RegistrarRespuestaVisitante();
    }

    public void RegistrarVisitanteRechazado()
    {
        // Rechazar = CORRECTO
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

        // Calcular penalización por tarea pendiente
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
        textoVisitantes.text = visitanteCorrecto ? "1/1" : "0/1";
        textoDineroVisitantes.text = $"-{penalizacionVisitante}€";
        textoTareas.text = tareaCompletada ? "1/1" : "0/1";
        textoDineroTareas.text = $"-{penalizacionTarea}€";

        bool despedido = sueldoActual < umbralDespido;
        bool victoria = !despedido && nocheActual >= 5;

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

        textoDineroVisitantes.color = penalizacionVisitante > 0 ? Color.red : Color.green;
        textoDineroTareas.color = penalizacionTarea > 0 ? Color.red : Color.green;

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

        // Reiniciar visitante
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