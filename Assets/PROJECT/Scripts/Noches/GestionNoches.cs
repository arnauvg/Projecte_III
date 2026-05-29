using UnityEngine;
using System;

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

    // Variables internas
    private int sueldoActual;
    private int nocheActual = 1;
    private bool yaNocheTerminada = false;

    // Estadísticas de la noche
    private int visitantesAcertados = 0;
    private int totalVisitantes = 0;
    private int dineroPerdidoVisitantes = 0;
    private int tareasCompletadas = 0;
    private int totalTareas = 1; // Por ahora 1 tarea por noche
    private int dineroPerdidoTareas = 0;
    private bool tareaCompletada = false;

    void Start()
    {
        sueldoActual = sueldoBase;

        if (cronometro == null)
            cronometro = FindFirstObjectByType<CronometroNoche>();

        // Contar visitantes totales de esta noche
        ContarTotalVisitantes();

        Debug.Log($"🌙 NOCHE {nocheActual} - Sueldo: {sueldoActual}€");
    }

    void ContarTotalVisitantes()
    {
        if (EstadoVisitantes.Instancia != null)
        {
            // Esto depende de cómo tengas configurado EstadoVisitantes
            // Si usas lista plana:
            // totalVisitantes = 3; // o calcula según la noche

            // Por ahora manual:
            switch (nocheActual)
            {
                case 1: totalVisitantes = 3; break;
                case 2: totalVisitantes = 3; break;
                case 3: totalVisitantes = 2; break;
                default: totalVisitantes = 3; break;
            }
        }
    }

    // Llamado cuando el jugador acierta (deja pasar bueno O rechaza malo)
    public void RegistrarAcierto()
    {
        visitantesAcertados++;
        Debug.Log($"✅ Visitante acertado ({visitantesAcertados}/{totalVisitantes})");
    }

    // Llamado cuando el jugador falla (rechaza bueno O acepta malo)
    public void RegistrarFallo()
    {
        dineroPerdidoVisitantes += penalizacionVisitanteIncorrecto;
        sueldoActual -= penalizacionVisitanteIncorrecto;
        Debug.Log($"❌ Visitante fallado! -{penalizacionVisitanteIncorrecto}€");
    }

    // Llamado desde el minijuego al completar la tarea
    public void CompletarTarea()
    {
        tareaCompletada = true;
        tareasCompletadas = 1;
        Debug.Log("✅ Tarea completada!");
    }

    // La noche termina SOLO por el reloj (06:00 AM)
    public void TerminarNochePorTiempo()
    {
        if (yaNocheTerminada) return;
        yaNocheTerminada = true;

        Debug.Log("=== FIN DE LA NOCHE (06:00 AM) ===");

        // Calcular penalización por tarea pendiente
        dineroPerdidoTareas = tareaCompletada ? 0 : penalizacionTareaPendiente;
        if (dineroPerdidoTareas > 0)
        {
            sueldoActual -= dineroPerdidoTareas;
            Debug.Log($"⚠️ Tarea pendiente! -{dineroPerdidoTareas}€");
        }

        // Mostrar pantalla de resultados
        MostrarPantallaFinNoche();
    }

    private void MostrarPantallaFinNoche()
    {
        bool gameOver = sueldoActual < umbralDespido;
        bool victoria = !gameOver && nocheActual >= 5;

        Action onContinue = () =>
        {
            if (gameOver || victoria)
                ReiniciarJuego();
            else
                SiguienteNoche();
        };

        if (uiFinNocheController != null)
        {
            uiFinNocheController.MostrarResultados(
                noche: nocheActual,
                visitantesAcertados: visitantesAcertados,
                totalVisitantes: totalVisitantes,
                dineroPerdidoVisitantes: dineroPerdidoVisitantes,
                tareasCompletadas: tareasCompletadas,
                totalTareas: totalTareas,
                dineroPerdidoTareas: dineroPerdidoTareas,
                sueldoActual: sueldoActual,
                gameOver: gameOver,
                onContinue: onContinue
            );
        }
    }

    private void SiguienteNoche()
    {
        nocheActual++;
        ReiniciarVariablesNoche();

        if (uiFinNocheController != null)
            uiFinNocheController.Ocultar();

        if (cronometro != null)
            cronometro.ReiniciarCronometro();

        ContarTotalVisitantes();

        Debug.Log($"🌙 NOCHE {nocheActual} - Sueldo: {sueldoActual}€");
    }

    private void ReiniciarJuego()
    {
        nocheActual = 1;
        sueldoActual = sueldoBase;
        ReiniciarVariablesNoche();

        if (uiFinNocheController != null)
            uiFinNocheController.Ocultar();

        if (cronometro != null)
            cronometro.ReiniciarCronometro();

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
}