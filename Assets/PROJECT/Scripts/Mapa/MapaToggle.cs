using UnityEngine;
using UnityEngine.SceneManagement;

public class MapaToggle : MonoBehaviour
{
    public GameObject panelMapa;

    private bool mapaAbierto = false;

    void Start()
    {
        panelMapa.SetActive(false); // empieza cerrado
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
        panelMapa.SetActive(true);
        mapaAbierto = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        

        Time.timeScale = 0f; // pausa el juego
    }

    void CerrarMapa()
    {
        panelMapa.SetActive(false);
        mapaAbierto = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f; // reanuda el juego
    }
    public void IrAEscena(string nombreEscena)
    {
        Time.timeScale = 1f; // por si el juego estaba pausado
        SceneManager.LoadScene(nombreEscena);
    }
}