using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GestionNoches : MonoBehaviour
{
    [Header("Configuración del Sistema")]
    public int sueldoBase = 1500;
    public int umbralDespido = 300;

    [Header("Penalizaciones")]
    public int penalizacionVisitanteIncorrecto = 50;
    public int penalizacionTareaFallada = 100;

    [Header("Referencias UI")]
    public GameObject pantallaFinNoche;  // Tu Canvas
    public TextMeshProUGUI textoNoche;
    public TextMeshProUGUI textoEstado;
    public TextMeshProUGUI textoNumVisitantes;
    public TextMeshProUGUI textoDineroVisitantes;
    public TextMeshProUGUI textoNumTareas;
    public TextMeshProUGUI textoDineroTareas;
    public Button botonSiguiente;
    public TextMeshProUGUI textoBoton;

    [Header("Referencias del Juego")]
    public GestorVisitantes gestorVisitantes;
    public GameObject botonVerde;  // El botón 3D de aceptar
    public GameObject botonRojo;   // El botón 3D de rechazar

    // Variables internas
    private int sueldoActual;
    private int nocheActual = 1;
    private int visitantesAtendidos = 0;
    private int visitantesCorrectos = 0;
    private int visitantesIncorrectos = 0;
    private int tareasCompletadas = 0;
    private bool nocheTerminada = false;
    private bool esperandoTransicion = false;

    void Start()
    {
        sueldoActual = sueldoBase;
        pantallaFinNoche.SetActive(false);

        Debug.Log($"=== NOCHE {nocheActual} COMENZADA ===");
        Debug.Log($"Sueldo inicial: {sueldoActual}€ | Umbral despido: {umbralDespido}€");
    }

    // Este método lo llamará GestorVisitantes cuando un visitante sea aceptado
    public void RegistrarVisitanteAceptado()
    {
        if (nocheTerminada || esperandoTransicion) return;

        visitantesAtendidos++;
        visitantesCorrectos++;
        Debug.Log($"✅ Visitante ACEPTADO correctamente ({visitantesCorrectos}/3)");

        VerificarFinNoche();
    }

    // Este método lo llamará GestorVisitantes cuando un visitante sea rechazado
    public void RegistrarVisitanteRechazado()
    {
        if (nocheTerminada || esperandoTransicion) return;

        visitantesAtendidos++;
        visitantesIncorrectos++;

        int perdida = penalizacionVisitanteIncorrecto;
        sueldoActual -= perdida;

        Debug.Log($"❌ Visitante RECHAZADO incorrectamente - ¡Pierdes {perdida}€! (Sueldo: {sueldoActual}€)");

        VerificarFinNoche();
    }

    // Método para tareas (lo llamarás cuando completes una tarea)
    public void CompletarTarea()
    {
        if (nocheTerminada || esperandoTransicion) return;

        if (tareasCompletadas >= 2)
        {
            Debug.Log("Ya has completado las 2 tareas de esta noche.");
            return;
        }

        tareasCompletadas++;
        Debug.Log($"📋 Tarea completada ({tareasCompletadas}/2)");

        VerificarFinNoche();
    }

    void VerificarFinNoche()
    {
        // Verificar si ya atendimos los 3 visitantes Y completamos las 2 tareas
        if (visitantesAtendidos >= 3 && tareasCompletadas >= 2 && !nocheTerminada)
        {
            TerminarNoche();
        }
    }

    void TerminarNoche()
    {
        nocheTerminada = true;
        esperandoTransicion = true;

        // Calcular penalizaciones
        int penalizacionVisitantes = visitantesIncorrectos * penalizacionVisitanteIncorrecto;
        int tareasFalladas = 2 - tareasCompletadas;
        int penalizacionTareas = tareasFalladas * penalizacionTareaFallada;
        int penalizacionTotal = penalizacionVisitantes + penalizacionTareas;

        int sueldoAntes = sueldoActual;
        sueldoActual -= penalizacionTotal;

        // Mostrar resultados en consola
        Debug.Log($"=== FIN DE LA NOCHE {nocheActual} ===");
        Debug.Log($"Visitantes correctos: {visitantesCorrectos}/3 (Penalización: -{penalizacionVisitantes}€)");
        Debug.Log($"Visitantes incorrectos: {visitantesIncorrectos}/3");
        Debug.Log($"Tareas completadas: {tareasCompletadas}/2 (Penalización: -{penalizacionTareas}€)");
        Debug.Log($"Penalización total: -{penalizacionTotal}€");
        Debug.Log($"Sueldo: {sueldoAntes}€ → {sueldoActual}€");

        // Verificar estado del juego
        bool despedido = sueldoActual < umbralDespido;
        bool victoria = false;

        if (despedido)
        {
            Debug.Log("💀 ¡TE HAN DESPEDIDO! 💀");
            MostrarPantallaFinNoche(true, false);
        }
        else if (nocheActual >= 5)
        {
            victoria = true;
            Debug.Log("🏆 ¡VICTORIA! Has completado las 5 noches 🏆");
            MostrarPantallaFinNoche(false, true);
        }
        else
        {
            Debug.Log($"✅ Pasas a la NOCHE {nocheActual + 1}");
            MostrarPantallaFinNoche(false, false);
        }
    }

    void MostrarPantallaFinNoche(bool despedido, bool victoria)
    {
        // Actualizar textos de la UI
        textoNoche.text = $"NOCHE {nocheActual}";

        if (despedido)
        {
            textoEstado.text = "GAME OVER";
            textoEstado.color = Color.red;
            textoBoton.text = "REINICIAR";
        }
        else if (victoria)
        {
            textoEstado.text = "¡VICTORIA!";
            textoEstado.color = Color.yellow;
            textoBoton.text = "JUGAR DE NUEVO";
        }
        else
        {
            textoEstado.text = "COMPLETADA";
            textoEstado.color = Color.green;
            textoBoton.text = "SIGUIENTE NOCHE";
        }

        // Mostrar estadísticas
        int penalizacionVisitantes = visitantesIncorrectos * penalizacionVisitanteIncorrecto;
        int tareasFalladas = 2 - tareasCompletadas;
        int penalizacionTareas = tareasFalladas * penalizacionTareaFallada;

        textoNumVisitantes.text = $"{visitantesCorrectos}/3";
        textoDineroVisitantes.text = $"-{penalizacionVisitantes} €";
        textoNumTareas.text = $"{tareasCompletadas}/2";
        textoDineroTareas.text = $"-{penalizacionTareas} €";

        // Cambiar colores según penalización
        textoDineroVisitantes.color = penalizacionVisitantes > 0 ? Color.red : Color.green;
        textoDineroTareas.color = penalizacionTareas > 0 ? Color.red : Color.green;

        // Configurar botón
        botonSiguiente.onClick.RemoveAllListeners();

        if (despedido || victoria)
        {
            botonSiguiente.onClick.AddListener(ReiniciarJuego);
        }
        else
        {
            botonSiguiente.onClick.AddListener(SiguienteNoche);
        }

        // Mostrar pantalla y pausar
        pantallaFinNoche.SetActive(true);
        Time.timeScale = 0f;

        // Desactivar botones de interacción
        if (botonVerde != null) botonVerde.SetActive(false);
        if (botonRojo != null) botonRojo.SetActive(false);
    }

    void SiguienteNoche()
    {
        nocheActual++;
        visitantesAtendidos = 0;
        visitantesCorrectos = 0;
        visitantesIncorrectos = 0;
        tareasCompletadas = 0;
        nocheTerminada = false;
        esperandoTransicion = false;

        pantallaFinNoche.SetActive(false);
        Time.timeScale = 1f;

        // Reactivar botones
        if (botonVerde != null) botonVerde.SetActive(true);
        if (botonRojo != null) botonRojo.SetActive(true);

        Debug.Log($"\n=== NOCHE {nocheActual} COMENZADA ===");
        Debug.Log($"Sueldo actual: {sueldoActual}€ | Umbral despido: {umbralDespido}€");

        // Llamar al siguiente visitante
        if (gestorVisitantes != null)
        {
            // Reiniciar el gestor para la nueva noche
            gestorVisitantes.ReiniciarNoche();
        }
    }

    void ReiniciarJuego()
    {
        Debug.Log("🔄 REINICIANDO JUEGO...");

        // Resetear variables
        nocheActual = 1;
        sueldoActual = sueldoBase;
        visitantesAtendidos = 0;
        visitantesCorrectos = 0;
        visitantesIncorrectos = 0;
        tareasCompletadas = 0;
        nocheTerminada = false;
        esperandoTransicion = false;

        pantallaFinNoche.SetActive(false);
        Time.timeScale = 1f;

        // Reactivar botones
        if (botonVerde != null) botonVerde.SetActive(true);
        if (botonRojo != null) botonRojo.SetActive(true);

        Debug.Log($"=== JUEGO REINICIADO ===");
        Debug.Log($"Noche {nocheActual} | Sueldo: {sueldoActual}€");

        // Reiniciar el gestor de visitantes
        if (gestorVisitantes != null)
        {
            gestorVisitantes.ReiniciarJuego();
        }
    }

    // ========== MÉTODOS PÚBLICOS PARA CONSULTAR ESTADO ==========
    // ¡ESTOS MÉTODOS DEBEN ESTAR DENTRO DE LA CLASE!

    public bool NocheTerminada()
    {
        return nocheTerminada || esperandoTransicion;
    }

    public bool EstaNocheActiva()
    {
        return !nocheTerminada && !esperandoTransicion;
    }

    void Update()
    {
        // TECLA F1: Forzar fin de noche (para pruebas)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("=== FORZANDO FIN DE NOCHE (MODO PRUEBA) ===");
            visitantesAtendidos = 3;
            tareasCompletadas = 2;
            VerificarFinNoche();
        }

        // TECLA F2: Ver estado actual
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log($"ESTADO: Visitantes={visitantesAtendidos}/3, Tareas={tareasCompletadas}/2, Sueldo={sueldoActual}€");
        }
    }
}