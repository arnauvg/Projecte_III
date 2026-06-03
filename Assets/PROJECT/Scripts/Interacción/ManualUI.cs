using UnityEngine;
using UnityEngine.UI;

public class ManualUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject canvasManual;
    public Image imagenPagina;
    public Sprite[] paginas;
    public Button botonAnterior;
    public Button botonSiguiente;

    [Header("Objeto con los scripts a desactivar")]
    public GameObject objetoConScripts;

    private int paginaActual = 0;
    private bool abierto = false;
    private MonoBehaviour[] scriptsADesactivar;

    void Start()
    {
        if (canvasManual != null)
            canvasManual.SetActive(false);
        MostrarPagina();

        if (objetoConScripts != null)
        {
            var lista = new System.Collections.Generic.List<MonoBehaviour>();
            MouseLook360 ml = objetoConScripts.GetComponent<MouseLook360>();
            if (ml != null) lista.Add(ml);
            InteraccionJugador ij = objetoConScripts.GetComponent<InteraccionJugador>();
            if (ij != null) lista.Add(ij);
            ClickCentro cc = objetoConScripts.GetComponent<ClickCentro>();
            if (cc != null) lista.Add(cc);
            scriptsADesactivar = lista.ToArray();
        }

        // Asegurar que el canvas manual está en la raíz del PersistentGameManager
        if (canvasManual != null && canvasManual.transform.parent != null)
        {
            // Si el canvasManual está dentro de PersistentGameManager, no hay problema
            Debug.Log($"ManualUI: canvasManual en {canvasManual.transform.parent.name}");
        }
    }

    public void Abrir()
    {
        if (abierto) return;
        abierto = true;

        if (canvasManual != null)
            canvasManual.SetActive(true);

        paginaActual = 0;
        MostrarPagina();

        // BLOQUEAR MAPA AL ABRIR MANUAL
        if (TareaManager.Instance != null)
            TareaManager.Instance.BloquearMapaPorTarea();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        foreach (var script in scriptsADesactivar)
            if (script != null) script.enabled = false;

        Debug.Log("📖 Manual abierto");
    }

    public void Cerrar()
    {
        if (!abierto) return;
        abierto = false;

        if (canvasManual != null)
            canvasManual.SetActive(false);

        // DESBLOQUEAR MAPA AL CERRAR MANUAL
        if (TareaManager.Instance != null)
            TareaManager.Instance.DesbloquearMapaPorTarea();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        foreach (var script in scriptsADesactivar)
            if (script != null)
            {
                script.enabled = true;
            }

        Debug.Log("📖 Manual cerrado");
    }

    public void SiguientePagina()
    {
        if (paginaActual < paginas.Length - 1)
        {
            paginaActual++;
            MostrarPagina();
        }
    }

    public void AnteriorPagina()
    {
        if (paginaActual > 0)
        {
            paginaActual--;
            MostrarPagina();
        }
    }

    private void MostrarPagina()
    {
        if (paginas.Length > 0 && imagenPagina != null)
            imagenPagina.sprite = paginas[paginaActual];

        if (botonAnterior != null)
            botonAnterior.interactable = (paginaActual > 0);
        if (botonSiguiente != null)
            botonSiguiente.interactable = (paginaActual < paginas.Length - 1);
    }
}