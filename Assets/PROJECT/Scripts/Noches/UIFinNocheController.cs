using UnityEngine;
using UnityEngine.UIElements;
using System;

public class UIFinNocheController : MonoBehaviour
{
    [Header("Referències UI Document")]
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
            Debug.LogError("UIFinNocheController: No s'ha trobat cap UIDocument al GameObject.");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        labelNoche = root.Q<Label>("noche");
        labelEstado = root.Q<Label>("estado");
        labelNumVisitantes = root.Q<Label>("num-visitantes");
        labelDineroVisitantes = root.Q<Label>("dinero-visitantes");
        labelNumTareas = root.Q<Label>("num-tareas");
        labelDineroTareas = root.Q<Label>("dinero-tareas");
        labelSueldo = root.Q<Label>("sueldo");
        botonContinuar = root.Q<Button>("boton-siguiente");

        if (labelNoche == null) Debug.LogWarning("No s'ha trobat l'element 'noche'.");
        if (labelEstado == null) Debug.LogWarning("No s'ha trobat l'element 'estado'.");

        // Ocultar al principi
        if (root != null)
            root.style.display = DisplayStyle.None;
    }

    public void MostrarResultados(
        int noche,
        bool visitanteCorrecto,
        int penalizacionVisitante,
        bool tareaCompletada,
        int penalizacionTarea,
        int sueldoActual,
        Action onContinue)
    {
        VisualElement root = uiDocument?.rootVisualElement;
        if (root == null) return;

        root.style.display = DisplayStyle.Flex;

        if (labelNoche != null) labelNoche.text = $"NOCHE {noche}";

        string estat = "COMPLETADA";
        if (penalizacionVisitante > 0) estat = "INCORRECTE";
        if (labelEstado != null) labelEstado.text = estat;

        if (labelNumVisitantes != null) labelNumVisitantes.text = visitanteCorrecto ? "1/1" : "0/1";
        if (labelDineroVisitantes != null)
        {
            labelDineroVisitantes.text = $"-{penalizacionVisitante}€";
            labelDineroVisitantes.style.color = penalizacionVisitante > 0 ? Color.red : Color.green;
        }

        if (labelNumTareas != null) labelNumTareas.text = tareaCompletada ? "1/1" : "0/1";
        if (labelDineroTareas != null)
        {
            labelDineroTareas.text = $"-{penalizacionTarea}€";
            labelDineroTareas.style.color = penalizacionTarea > 0 ? Color.red : Color.green;
        }

        if (labelSueldo != null) labelSueldo.text = $"{sueldoActual}€";

        // Netejar listeners anteriors
        if (botonContinuar != null && onContinueAction != null)
            botonContinuar.clicked -= onContinueAction;

        onContinueAction = onContinue;

        if (botonContinuar != null)
            botonContinuar.clicked += ExecutarContinuar;
    }

    public void Ocultar()
    {
        if (uiDocument?.rootVisualElement != null)
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void ExecutarContinuar()
    {
        Ocultar();
        onContinueAction?.Invoke();
    }
}