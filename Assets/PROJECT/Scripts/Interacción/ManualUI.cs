using UnityEngine;
using UnityEngine.UI;

public class ManualUI : MonoBehaviour
{
    public GameObject canvasManual;
    public Image imagenPagina;
    public Sprite[] paginas;
    public MonoBehaviour[] scriptsADesactivar; // ← arrastra aquí MouseLook, InteraccionJugador, ClickCentro

    private int paginaActual = 0;
    private bool abierto = false;

    void Start()
    {
        canvasManual.SetActive(false);
        MostrarPagina();
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

        // Desactivar todos los scripts de control
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

        // Reactivar scripts
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

    void MostrarPagina()
    {
        if (paginas.Length > 0 && imagenPagina != null)
            imagenPagina.sprite = paginas[paginaActual];
    }
}