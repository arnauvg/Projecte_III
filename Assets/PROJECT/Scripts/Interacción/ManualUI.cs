using UnityEngine;
using UnityEngine.UI;

public class ManualUI : MonoBehaviour
{
    public GameObject canvasManual;
    public MonoBehaviour controladorJugador;
    public Image imagenPagina;
    public Sprite[] paginas;
    private int paginaActual = 0;

    void Start()
    {
        canvasManual.SetActive(false);
        MostrarPagina();
    }

    public void Abrir()
    {
        canvasManual.SetActive(true);
        paginaActual = 0;
        MostrarPagina();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (controladorJugador != null) controladorJugador.enabled = false;
        
    }

    public void Cerrar()
    {
        canvasManual.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (controladorJugador != null) controladorJugador.enabled = true;
        
    }

    public void SiguientePagina() { if (paginaActual < paginas.Length - 1) { paginaActual++; MostrarPagina(); } }
    public void AnteriorPagina() { if (paginaActual > 0) { paginaActual--; MostrarPagina(); } }
    void MostrarPagina() { if (paginas.Length > 0 && imagenPagina != null) imagenPagina.sprite = paginas[paginaActual]; }
}