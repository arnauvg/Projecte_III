using UnityEngine;

public class CerrarMinijuego : MonoBehaviour
{
    public GameObject canvasMinijuego;
    public AudioClip sonidoCompletado;

    private AudioSource audioSource;
    private TareaManager tareaManager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        tareaManager = FindFirstObjectByType<TareaManager>();
    }

    public void CompletarYCerrar()
    {
        if (sonidoCompletado != null && audioSource != null)
            audioSource.PlayOneShot(sonidoCompletado);

        if (tareaManager != null)
            tareaManager.CompletarTareaActual();

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
            uiManager.MarcarTareaCompletada();

        Invoke(nameof(Cerrar), 0.3f);
    }

    void Cerrar()
    {
        if (canvasMinijuego != null)
            canvasMinijuego.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}