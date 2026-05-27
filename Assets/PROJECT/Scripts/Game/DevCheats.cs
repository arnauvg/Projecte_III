using UnityEngine;

public class DevCheats : MonoBehaviour
{
    [Header("Tecla para saltar tutorial")]
    public KeyCode skipKey = KeyCode.F1;

    private DialogueManager dialogueManager;
    private Telefono telefono;

    void Start()
    {
        // Buscar automáticamente los componentes en la escena
        dialogueManager = FindObjectOfType<DialogueManager>();
        telefono = FindObjectOfType<Telefono>();

        if (dialogueManager == null)
            Debug.LogWarning("DevCheats: No se encontró DialogueManager en la escena.");
        if (telefono == null)
            Debug.LogWarning("DevCheats: No se encontró Telefono en la escena.");
    }

    void Update()
    {
        if (Input.GetKeyDown(skipKey))
        {
            SkipTutorial();
        }
    }

    private void SkipTutorial()
    {
        Debug.Log("DevCheats: Saltando tutorial (teléfono + diálogo).");

        // 1. Silenciar y marcar teléfono como ya activado
        if (telefono != null)
            telefono.SkipPhone();

        // 2. Forzar fin del diálogo si está activo
        if (dialogueManager != null)
            dialogueManager.ForceEndDialogue();
    }
}