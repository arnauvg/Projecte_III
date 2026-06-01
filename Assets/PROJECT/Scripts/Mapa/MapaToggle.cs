using UnityEngine;
using UnityEngine.SceneManagement;

public class MapaToggle : MonoBehaviour
{
    public GameObject panelMapa;
    private bool mapaAbierto = false;

    void Start()
    {
        if (panelMapa == null)
            panelMapa = GameObject.Find("PanelMapa");

        if (panelMapa != null) panelMapa.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (mapaAbierto)
                CerrarMapa();
            else
                AbrirMapa();
        }
    }

    void AbrirMapa()
    {
        if (panelMapa != null)
            panelMapa.SetActive(true);

        mapaAbierto = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    void CerrarMapa()
    {
        if (panelMapa != null)
            panelMapa.SetActive(false);

        mapaAbierto = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void IrAEscena(string nombreEscena)
    {
        // Cerrar el mapa si está abierto
        if (mapaAbierto)
        {
            CerrarMapa();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}