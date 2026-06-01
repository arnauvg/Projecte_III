using UnityEngine;

public class DevCheats : MonoBehaviour
{
    [Header("Trucos - Teclas editables")]
    public KeyCode skipTutorialKey = KeyCode.F1;
    public KeyCode forceNextNightKey = KeyCode.F2;
    public KeyCode forceGameOverKey = KeyCode.F3;

    private DialogueManager dialogueManager;
    private Telefono telefono;
    private GestionNoches gestionNoches;

    void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        telefono = FindFirstObjectByType<Telefono>();
        gestionNoches = FindFirstObjectByType<GestionNoches>();

        if (dialogueManager == null)
            Debug.LogWarning("DevCheats: No se encontró DialogueManager.");
        if (telefono == null)
            Debug.LogWarning("DevCheats: No se encontró Telefono.");
        if (gestionNoches == null)
            Debug.LogWarning("DevCheats: No se encontró GestionNoches.");
    }

    void Update()
    {
        if (Input.GetKeyDown(skipTutorialKey))
            SkipTutorial();

        if (Input.GetKeyDown(forceNextNightKey))
            ForceNextNight();

        if (Input.GetKeyDown(forceGameOverKey))
            ForceGameOver();
    }

    private void SkipTutorial()
    {
        Debug.Log("DevCheats: Saltando tutorial (teléfono + diálogo).");
        if (telefono != null)
            telefono.SkipPhone();
        if (dialogueManager != null)
            dialogueManager.ForceEndDialogue();
    }

    private void ForceNextNight()
    {
        Debug.Log("DevCheats: Forzando fin de noche con éxito (siguiente noche).");
        if (gestionNoches != null)
            gestionNoches.ForceNightComplete();
        else
            Debug.LogWarning("DevCheats: GestionNoches no encontrado.");
    }

    private void ForceGameOver()
    {
        Debug.Log("DevCheats: Forzando Game Over.");
        if (gestionNoches != null)
            gestionNoches.ForceGameOver();
        else
            Debug.LogWarning("DevCheats: GestionNoches no encontrado.");
    }
}