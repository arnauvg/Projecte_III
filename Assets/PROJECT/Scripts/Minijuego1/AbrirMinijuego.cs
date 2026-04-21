using UnityEngine;

public class AbrirMinijuego : MonoBehaviour
{
    public GameObject canvasMinijuego;

    void OnMouseDown()
    {
        canvasMinijuego.SetActive(true);

        // Opcional: desbloquear cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Opcional: pausar jugador (luego lo hacemos mejor si quieres)
        Time.timeScale = 0f;
    }
}