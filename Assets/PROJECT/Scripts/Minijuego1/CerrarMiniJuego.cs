using UnityEngine;

public class CerrarMinijuego : MonoBehaviour
{
    public GameObject canvasMinijuego;

    public void Cerrar()
    {
        // Cerrar el canvas
        canvasMinijuego.SetActive(false);

        // Volver al modo juego (FPS)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reanudar el tiempo
        Time.timeScale = 1f;
    }
}