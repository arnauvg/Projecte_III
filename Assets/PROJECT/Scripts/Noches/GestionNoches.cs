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

    [Header("UI (UI Toolkit)")]
    public UIFinNocheController uiFinNocheController;   // ← NUEVO: controlador de la UI con UI Toolkit

    [Header("Referencias (opcional, solo si usas Canvas antiguo)")]
    public GestorVisitantesSimple gestorVisitantes;

    // Variables internas
    private int sueldoActual;
    private int nocheActual = 1;
    private bool visitanteCorrecto = false;
    private bool tareaCompletada = false;
    private int penalizacionVisitante = 0;

    void Start()
    {
        sueldoActual = sueldoBase;
        Debug.Log($"NOCHE {nocheActual} - Sueldo: {sueldoActual}€");
    }

    // Llamado desde VisitanteSimple cuando pulsan VERDE (INCORRECTO)
    public void RegistrarVisitanteAceptado()
    {
        penalizacionVisitante = penalizacionVisitanteIncorrecto;
        visitanteCorrecto = false;
        sueldoActual -= penalizacionVisitante;
        Debug.Log($"Visitante INCORRECTO! -{penalizacionVisitante}€");

        if (gestorVisitantes != null)
            gestorVisitantes.RegistrarRespuestaVisitante();
    }

    // Llamado desde VisitanteSimple cuando pulsan ROJO (CORRECTO)
    public void RegistrarVisitanteRechazado()
    {
        penalizacionVisitante = 0;
        visitanteCorrecto = true;
        Debug.Log($"Visitante CORRECTO! Sin penalización");

        if (gestorVisitantes != null)
            gestorVisitantes.RegistrarRespuestaVisitante();
    }

    // Llamado desde el minijuego de velas al completarlo
    public void CompletarTarea()
    {
        tareaCompletada = true;
        Debug.Log("Tarea completada!");
    }

    // Llamado por GestorVisitantes cuando el visitante termina de salir
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

    // Aquí se muestra la pantalla usando UI Toolkit (ya no usa Canvas)
    private void MostrarPantallaFinNoche(int penalizacionTarea)
    {
        bool despedido = sueldoActual < umbralDespido;
        bool victoria = !despedido && nocheActual >= 5;

        // Acción que se ejecutará cuando el usuario pulse el botón
        System.Action onContinue = () =>
        {
            if (despedido || victoria)
                ReiniciarJuego();
            else
                SiguienteNoche();
        };

        // Mostrar la pantalla con el controlador de UI Toolkit
        uiFinNocheController.MostrarResultados(
            noche: nocheActual,
            visitanteCorrecto: visitanteCorrecto,
            penalizacionVisitante: penalizacionVisitante,
            tareaCompletada: tareaCompletada,
            penalizacionTarea: penalizacionTarea,
            sueldoActual: sueldoActual,
            onContinue: onContinue
        );
    }

    private void SiguienteNoche()
    {
        nocheActual++;
        visitanteCorrecto = false;
        tareaCompletada = false;
        penalizacionVisitante = 0;

        // Ocultamos la pantalla
        uiFinNocheController.Ocultar();

        // Reanudar el tiempo (por si estaba pausado)
        Time.timeScale = 1f;

        // Reiniciar el visitante para que vuelva a aparecer
        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
        if (visitante != null)
            visitante.ReiniciarParaNuevaNoche();

        Debug.Log($"NOCHE {nocheActual} - Sueldo: {sueldoActual}€");
    }

    private void ReiniciarJuego()
    {
        nocheActual = 1;
        sueldoActual = sueldoBase;
        visitanteCorrecto = false;
        tareaCompletada = false;
        penalizacionVisitante = 0;

        uiFinNocheController.Ocultar();

        Time.timeScale = 1f;

        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
        if (visitante != null)
            visitante.ReiniciarParaNuevaNoche();

        Debug.Log("JUEGO REINICIADO");
    }
}