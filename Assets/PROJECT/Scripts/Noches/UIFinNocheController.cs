using UnityEngine;
using UnityEngine.UIElements;
using System;

public class UIFinNocheController : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    private Label labelNoche;
    private Label labelEstado;
    private Label labelNumVisitantes;
    private Label labelDineroVisitantes;
    private Label labelNumTareas;
    private Label labelDineroTareas;
    private Label labelSueldo;
    private Button botonContinuar;

    private Action onContinueAction;

    void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("UIFinNocheController: No se encontró UIDocument");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;

        // Buscar todos los elementos por nombre
        labelNoche = root.Q<Label>("noche");
        labelEstado = root.Q<Label>("estado");
        labelNumVisitantes = root.Q<Label>("num-visitantes");
        labelDineroVisitantes = root.Q<Label>("dinero-visitantes");
        labelNumTareas = root.Q<Label>("num-tareas");
        labelDineroTareas = root.Q<Label>("dinero-tareas");
        labelSueldo = root.Q<Label>("sueldo");
        botonContinuar = root.Q<Button>("boton-siguiente");

        // Ocultar al inicio
        if (root != null)
            root.style.display = DisplayStyle.None;
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
        Action onContinue)
    {
        VisualElement root = uiDocument?.rootVisualElement;
        if (root == null) return;

        root.style.display = DisplayStyle.Flex;

        // NOCHE
        if (labelNoche != null)
            labelNoche.text = $"NOCHE {noche}";

        // ESTADO (Completada / Game Over)
        if (labelEstado != null)
        {
            if (gameOver)
                labelEstado.text = "GAME OVER";
            else
                labelEstado.text = "COMPLETADA";
        }

        // VISITANTES ACERTADOS
        if (labelNumVisitantes != null)
            labelNumVisitantes.text = $"{visitantesAcertados}/{totalVisitantes}";

        // DINERO PERDIDO POR VISITANTES
        if (labelDineroVisitantes != null)
        {
            labelDineroVisitantes.text = $"-{dineroPerdidoVisitantes}€";
            labelDineroVisitantes.style.color = dineroPerdidoVisitantes > 0 ? Color.red : Color.green;
        }

        // TAREAS COMPLETADAS
        if (labelNumTareas != null)
            labelNumTareas.text = $"{tareasCompletadas}/{totalTareas}";

        // DINERO PERDIDO POR TAREAS
        if (labelDineroTareas != null)
        {
            labelDineroTareas.text = $"-{dineroPerdidoTareas}€";
            labelDineroTareas.style.color = dineroPerdidoTareas > 0 ? Color.red : Color.green;
        }

        // SUELDO TOTAL
        if (labelSueldo != null)
            labelSueldo.text = $"{sueldoActual}€";

        // Guardar acción para el botón
        onContinueAction = onContinue;

        // Configurar botón
        if (botonContinuar != null)
        {
            botonContinuar.clicked -= EjecutarContinuar;
            botonContinuar.clicked += EjecutarContinuar;
        }
    }

    public void Ocultar()
    {
        if (uiDocument?.rootVisualElement != null)
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void EjecutarContinuar()
    {
        Ocultar();
        onContinueAction?.Invoke();
    }
}