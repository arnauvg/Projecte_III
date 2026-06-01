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
        labelNoche = root.Q<Label>("noche");
        labelEstado = root.Q<Label>("estado");
        labelNumVisitantes = root.Q<Label>("num-visitantes");
        labelDineroVisitantes = root.Q<Label>("dinero-visitantes");
        labelNumTareas = root.Q<Label>("num-tareas");
        labelDineroTareas = root.Q<Label>("dinero-tareas");
        labelSueldo = root.Q<Label>("sueldo");
        botonContinuar = root.Q<Button>("boton-siguiente");

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
        bool victoria,
        string mensajeEstado,
        Action onContinue)
    {
        VisualElement root = uiDocument?.rootVisualElement;
        if (root == null) return;

        root.style.display = DisplayStyle.Flex;

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

        // 🔥 Usar UnityEngine.Cursor explícitamente
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