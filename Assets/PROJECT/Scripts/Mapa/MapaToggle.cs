using UnityEngine;
using UnityEngine.SceneManagement;

public class MapaToggle : MonoBehaviour
{
    public GameObject panelMapa;
    private bool mapaAbierto = false;
    private MouseLook360 mouseLook;

    void Start()
    {
        if (panelMapa == null)
            panelMapa = GameObject.Find("PanelMapa");

        if (panelMapa != null) panelMapa.SetActive(false);

        mouseLook = FindFirstObjectByType<MouseLook360>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!mapaAbierto && TareaManager.mapaBloqueadoPorTarea)
            {
                Debug.Log("No puedes abrir el mapa mientras hay una tarea abierta");
                return;
            }

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

        if (mouseLook != null) mouseLook.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    void CerrarMapa()
    {
        if (panelMapa != null)
            panelMapa.SetActive(false);

        mapaAbierto = false;

        if (mouseLook != null) mouseLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void IrAEscena(string nombreEscena)
    {
        Debug.Log($"IrAEscena llamado: {nombreEscena}, mapaAbierto={mapaAbierto}");

        // Cerrar el mapa si está abierto
        if (mapaAbierto)
        {
            // Desactivar el panel del mapa visualmente
            if (panelMapa != null)
                panelMapa.SetActive(false);

            mapaAbierto = false;

            // Reactivar control de cámara (aunque luego se cargará otra escena)
            if (mouseLook != null) mouseLook.enabled = true;
        }

        // Asegurar que el tiempo está normalizado
        Time.timeScale = 1f;

        // Cargar la nueva escena
        SceneManager.LoadScene(nombreEscena);
    }
}