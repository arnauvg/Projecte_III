using UnityEngine;

public class LibroManual : Interactuable
{
    // Ya no necesitas asignar manualUI en el Inspector
    private ManualUI manualUI;

    void Start()
    {
        // Buscar ManualUI al inicio
        BuscarManualUI();
    }

    void BuscarManualUI()
    {
        // Buscar ManualUI en toda la escena (incluyendo objetos desactivados)
        manualUI = FindFirstObjectByType<ManualUI>(FindObjectsInactive.Include);

        if (manualUI == null)
        {
            Debug.LogWarning("ManualUI no encontrado, reintentando en 0.5 segundos...");
            Invoke(nameof(BuscarManualUI), 0.5f);
        }
        else
        {
            Debug.Log("ManualUI encontrado: " + manualUI.name);
        }
    }

    void OnMouseDown()
    {
        Debug.Log("📖 Click en el manual");
        Recoger();
    }

    public override bool Recoger()
    {
        if (manualUI == null)
        {
            BuscarManualUI();
            if (manualUI == null)
            {
                Debug.LogError("No se pudo encontrar ManualUI");
                return false;
            }
        }

        Debug.Log($"Abriendo manual. manualUI = {(manualUI != null ? "asignado" : "NULL")}");
        manualUI.Abrir();

        if (TareaManager.Instance != null)
            TareaManager.Instance.BloquearMapaPorTarea();
        return false;
    }

    public override void Soltar() { }
}