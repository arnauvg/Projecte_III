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

        // 🔥 Desactivar el control de la cámara
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

        // 🔥 Reactivar el control de la cámara
        if (mouseLook != null) mouseLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void IrAEscena(string nombreEscena)
    {
        if (mapaAbierto)
            CerrarMapa();

        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}