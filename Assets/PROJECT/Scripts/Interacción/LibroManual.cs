using UnityEngine;

public class LibroManual : Interactuable
{
    public ManualUI manualUI;

    void OnMouseDown()
    {
        Debug.Log("📖 Click en el manual");
        Recoger();
    }

    public override bool Recoger()
    {
        Debug.Log($"Intentando abrir manual. manualUI = {(manualUI != null ? "asignado" : "NULL")}");
        if (manualUI != null)
        {
            manualUI.Abrir();
            Debug.Log("Manual abierto");
        }
        else
        {
            Debug.LogError("ManualUI no asignado en LibroManual");
        }
        return false;
    }

    public override void Soltar() { }
}