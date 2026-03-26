using UnityEngine;

public class Manual : Interactuable
{
    [Header("Configuración UI")]
    public GameObject panelManual;

    private bool manualAbierto = false;

    void Start()
    {
        if (panelManual != null)
            panelManual.SetActive(false);
    }

    public override bool Recoger()
    {
        if (!manualAbierto)
        {
            manualAbierto = true;
            panelManual.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            return true;
        }
        return false;
    }

    public override void Soltar()
    {
        if (manualAbierto)
        {
            manualAbierto = false;
            panelManual.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
}