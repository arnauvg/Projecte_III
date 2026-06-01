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
    public GameObject objetoConScripts; // ← arrastra aquí la Main Camera

    private int paginaActual = 0;
    private bool abierto = false;
    private MonoBehaviour[] scriptsADesactivar;

    void Start()
    {
        canvasManual.SetActive(false);
        MostrarPagina();

        // Buscar los scripts en el objeto asignado
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
    }

    public void Abrir()
    {
        if (abierto) return;
        abierto = true;

        canvasManual.SetActive(true);
        paginaActual = 0;
        MostrarPagina();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        foreach (var script in scriptsADesactivar)
            if (script != null) script.enabled = false;
    }

    public void Cerrar()
    {
        if (!abierto) return;
        abierto = false;

        canvasManual.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        foreach (var script in scriptsADesactivar)
            if (script != null) script.enabled = true;
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