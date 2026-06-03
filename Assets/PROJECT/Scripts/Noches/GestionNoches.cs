using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GestionNoches : MonoBehaviour
{
    [Header("Sueldo")]
    public int sueldoBase = 1500;
    public int umbralDespido = 300;

    [Header("Penalizaciones")]
    public int penalizacionVisitanteIncorrecto = 50;
    public int penalizacionTareaPendiente = 100;

    [Header("Referencias")]
    public UIFinNocheController uiFinNocheController;
    public CronometroNoche cronometro;
    public GestorVisitantesSimple gestorVisitantes;

    private int sueldoActual;
    private int nocheActual = 1;
    private bool yaNocheTerminada = false;

    private int visitantesAcertados = 0;
    private int totalVisitantes = 0;
    private int dineroPerdidoVisitantes = 0;
    private int tareasCompletadas = 0;
    private int totalTareas = 1;
    private int dineroPerdidoTareas = 0;
    private bool tareaCompletada = false;

    void Start()
    {
        sueldoActual = sueldoBase;
        if (cronometro == null) cronometro = FindFirstObjectByType<CronometroNoche>();
        if (gestorVisitantes == null) gestorVisitantes = FindFirstObjectByType<GestorVisitantesSimple>();
        ContarTotalVisitantes();
        Debug.Log($"🌙 NOCHE {nocheActual} - Sueldo: {sueldoActual}€");
    }

    void ContarTotalVisitantes()
    {
        totalVisitantes = (gestorVisitantes != null) ? gestorVisitantes.maxVisitantesPorNoche : 3;
    }

    public void RegistrarAcierto()
    {
        visitantesAcertados++;
        Debug.Log($"✅ Visitante acertado ({visitantesAcertados}/{totalVisitantes})");
    }

    public void RegistrarFallo()
    {
        dineroPerdidoVisitantes += penalizacionVisitanteIncorrecto;
        sueldoActual -= penalizacionVisitanteIncorrecto;
        Debug.Log($"❌ Visitante fallado! -{penalizacionVisitanteIncorrecto}€");
    }

    public void CompletarTarea()
    {
        tareaCompletada = true;
        tareasCompletadas = 1;
        Debug.Log("✅ Tarea completada!");
    }

    public void TerminarNochePorTiempo()
    {
        if (yaNocheTerminada) return;
        yaNocheTerminada = true;

        Debug.Log("=== FIN DE LA NOCHE (06:00 AM) ===");
        dineroPerdidoTareas = tareaCompletada ? 0 : penalizacionTareaPendiente;
        if (dineroPerdidoTareas > 0)
        {
            sueldoActual -= dineroPerdidoTareas;
            Debug.Log($"⚠️ Tarea pendiente! -{dineroPerdidoTareas}€");
        }
        MostrarPantallaFinNoche();
    }

    private void MostrarPantallaFinNoche()
    {
        bool gameOver = sueldoActual < umbralDespido;
        bool victoria = !gameOver && nocheActual >= 3;
        string mensajeEstado;
        if (gameOver) mensajeEstado = "GAME OVER";
        else if (victoria) mensajeEstado = "¡CONTRATO SUPERADO!";
        else mensajeEstado = "COMPLETADA";

        Action onContinue = () =>
        {
            if (gameOver) ReiniciarJuego();
            else if (victoria) VolverAlMenuPrincipal();
            else SiguienteNoche();
        };

        uiFinNocheController?.MostrarResultados(
            noche: nocheActual,
            visitantesAcertados: visitantesAcertados,
            totalVisitantes: totalVisitantes,
            dineroPerdidoVisitantes: dineroPerdidoVisitantes,
            tareasCompletadas: tareasCompletadas,
            totalTareas: totalTareas,
            dineroPerdidoTareas: dineroPerdidoTareas,
            sueldoActual: sueldoActual,
            gameOver: gameOver,
            victoria: victoria,
            mensajeEstado: mensajeEstado,
            onContinue: onContinue
        );
    }

    private void SiguienteNoche()
    {
        Debug.Log("=== SIGUIENTE NOCHE ===");

        nocheActual++;
        ReiniciarVariablesNoche();

        // Reiniciar el gestor de visitantes
        if (gestorVisitantes != null)
        {
            gestorVisitantes.ReiniciarNoche();
        }

        if (uiFinNocheController != null)
            uiFinNocheController.Ocultar();

        if (cronometro != null)
            cronometro.ReiniciarCronometro();

        ContarTotalVisitantes();

        Debug.Log($"🌙 NOCHE {nocheActual} - Sueldo: {sueldoActual}€");
    }

    private void ReiniciarJuego()
    {
        Telefono.Resetear();
        nocheActual = 1;
        sueldoActual = sueldoBase;
        ReiniciarVariablesNoche();
        if (gestorVisitantes != null) gestorVisitantes.ReiniciarJuegoCompleto();
        uiFinNocheController?.Ocultar();
        cronometro?.ReiniciarCronometro();
        ContarTotalVisitantes();
        Debug.Log("🔄 JUEGO REINICIADO");
    }

    private void ReiniciarVariablesNoche()
    {
        yaNocheTerminada = false;
        visitantesAcertados = 0;
        dineroPerdidoVisitantes = 0;
        tareasCompletadas = 0;
        dineroPerdidoTareas = 0;
        tareaCompletada = false;
    }

    private void VolverAlMenuPrincipal()
    {
        if (CronometroNoche.Instance != null) CronometroNoche.Instance.ReiniciarCronometro();
        SceneManager.LoadScene("MainMenu");
    }

    public int GetNocheActual() => nocheActual;

    // 🆕 Métodos para DevCheats
    public void ForceNightComplete()
    {
        if (yaNocheTerminada) return;
        if (sueldoActual < umbralDespido) sueldoActual = umbralDespido + 1;
        TerminarNochePorTiempo();
    }

    public void ForceGameOver()
    {
        if (yaNocheTerminada) return;
        sueldoActual = umbralDespido - 1;
        TerminarNochePorTiempo();
    }
}