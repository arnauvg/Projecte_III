using UnityEngine;

public class ManualInteractuable : MonoBehaviour
{
    public Manual manual;

    public void Interactuar()
    {
        if (manual != null)
        {
            manual.AbrirManual();
        }
    }
}