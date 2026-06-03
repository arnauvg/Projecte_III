using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public class UIFinNocheController : MonoBehaviour
{
    private UIDocument uiDocument;
    private Label labelNoche;
    private Label labelEstado;
    private Label labelNumVisitantes;
    private Label labelDineroVisitantes;
    private Label labelNumTareas;
    private Label labelDineroTareas;
    private Label labelSueldo;
    private Button botonContinuar;

    private Action onContinueAction;
    private bool uiConectado = false;

    void Awake()
    {
        // Obtener el UIDocument
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIFinNocheController: No se encontró UIDocument en el mismo GameObject");
            return;
        }

        // Esperar a que el UIDocument esté listo
        StartCoroutine(EsperarYConectarUI());
    }

    IEnumerator EsperarYConectarUI()
    {
        // Esperar un frame para que el UIDocument se inicialice
        yield return null;

        // Intentar conectar la UI
        ConectarUI();

        // Si falla, esperar otro frame (por si el Source Asset se carga)
        if (!uiConectado)
        {
            yield return null;
            ConectarUI();
        }
    }

    void ConectarUI()
    {
        if (uiDocument == null) return;

        VisualElement root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogWarning("UIFinNocheController: rootVisualElement es null, reintentando...");
            return;
        }

        labelNoche = root.Q<Label>("noche");
        labelEstado = root.Q<Label>("estado");
        labelNumVisitantes = root.Q<Label>("num-visitantes");
        labelDineroVisitantes = root.Q<Label>("dinero-visitantes");
        labelNumTareas = root.Q<Label>("num-tareas");
        labelDineroTareas = root.Q<Label>("dinero-tareas");
        labelSueldo = root.Q<Label>("sueldo");
        botonContinuar = root.Q<Button>("boton-siguiente");

        // Ocultar al inicio
        root.style.display = DisplayStyle.None;

        uiConectado = true;
        Debug.Log("UIFinNocheController: UI conectada correctamente");
    }

    public void MostrarResultados(
        int noche,
        int visitantesAcertados,
        int totalVisitantes,
        int dineroPerdidoVisitantes,
        int tareasCompletadas,
        int totalTareas,
        int dineroPerdidoTareas,
        int sueldoActual,
        bool gameOver,
        bool victoria,
        string mensajeEstado,
        Action onContinue)
    {
        // Si la UI no está conectada, intentar conectar
        if (!uiConectado)
        {
            ConectarUI();
            if (!uiConectado)
            {
                Debug.LogError("No se puede mostrar resultados: UI no disponible");
                return;
            }
        }

        if (uiDocument == null || uiDocument.rootVisualElement == null)
        {
            Debug.LogError("No se puede mostrar resultados: UI no disponible");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        root.style.display = DisplayStyle.Flex;
        Debug.Log("Mostrando pantalla de fin de noche");

        if (labelNoche != null) labelNoche.text = $"NOCHE {noche}";
        if (labelEstado != null) labelEstado.text = mensajeEstado;
        if (labelNumVisitantes != null) labelNumVisitantes.text = $"{visitantesAcertados}/{totalVisitantes}";
        if (labelDineroVisitantes != null)
        {
            labelDineroVisitantes.text = $"-{dineroPerdidoVisitantes}€";
            labelDineroVisitantes.style.color = dineroPerdidoVisitantes > 0 ? Color.red : Color.green;
        }
        if (labelNumTareas != null) labelNumTareas.text = $"{tareasCompletadas}/{totalTareas}";
        if (labelDineroTareas != null)
        {
            labelDineroTareas.text = $"-{dineroPerdidoTareas}€";
            labelDineroTareas.style.color = dineroPerdidoTareas > 0 ? Color.red : Color.green;
        }
        if (labelSueldo != null) labelSueldo.text = $"{sueldoActual}€";

        onContinueAction = onContinue;

        if (botonContinuar != null)
        {
            botonContinuar.text = victoria ? "VOLVER AL MENÚ" : "SIGUIENTE NOCHE";
            botonContinuar.clicked -= EjecutarContinuar;
            botonContinuar.clicked += EjecutarContinuar;
        }

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        Time.timeScale = 1f;
    }

    public void Ocultar()
    {
        if (uiDocument?.rootVisualElement != null)
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void EjecutarContinuar()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        Ocultar();
        onContinueAction?.Invoke();
    }
}