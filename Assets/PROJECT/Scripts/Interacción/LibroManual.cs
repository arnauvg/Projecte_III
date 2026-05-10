using UnityEngine;

public class LibroManual : Interactuable
{
    public ManualUI manualUI;

    public override bool Recoger()
    {
        if (manualUI != null) manualUI.Abrir();
        return false;
    }

    public override void Soltar() { }
}