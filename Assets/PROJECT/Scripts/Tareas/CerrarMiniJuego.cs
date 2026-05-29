using UnityEngine;
using System.Collections;

public class CerrarMinijuego : MonoBehaviour
{
    public GameObject canvasMinijuego;
    public AudioClip sonidoCompletado;

    private AudioSource audioSource;
    private TareaManager tareaManager;
    private bool completado = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.volume = 1f;

        tareaManager = FindFirstObjectByType<TareaManager>();
    }

    public void Cerrar()
    {
        Debug.Log("Cerrando minijuego");
        CerrarCanvas();
    }

    public void CompletarYCerrar()
    {
        if (completado) return;
        completado = true;

        Debug.Log("Completando tarea");

        // Notificar a TareaManager
        if (tareaManager != null)
            tareaManager.CompletarTareaActual();

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
            uiManager.MarcarTareaCompletada();

        // Reproducir sonido y esperar antes de cerrar
        StartCoroutine(ReproducirSonidoYCerrar());
    }

    IEnumerator ReproducirSonidoYCerrar()
    {
        // Reproducir sonido
        if (sonidoCompletado != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoCompletado, 1f);
            Debug.Log("🔊 Reproduciendo sonido de completado");

            // Esperar la duración del sonido
            yield return new WaitForSecondsRealtime(sonidoCompletado.length);
        }
        else
        {
            Debug.LogWarning("No hay sonido asignado, esperando 0.5 segundos");
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Cerrar después del sonido
        CerrarCanvas();
    }

    void CerrarCanvas()
    {
        Debug.Log("Cerrando canvas...");

        if (canvasMinijuego != null)
        {
            canvasMinijuego.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}