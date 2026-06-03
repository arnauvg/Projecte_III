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
        Debug.Log($"CerrarMinijuego iniciado en {gameObject.name}");
    }

    public void Cerrar()
    {
        Debug.Log("Cerrando minijuego sin completar");
        CerrarCanvas();
    }

    public void CompletarYCerrar()
    {
        if (completado) return;
        completado = true;

        Debug.Log("Completando tarea y cerrando minijuego");

        if (tareaManager != null)
            tareaManager.CompletarTareaActual();

        StartCoroutine(ReproducirSonidoYCerrar());
    }

    IEnumerator ReproducirSonidoYCerrar()
    {
        if (sonidoCompletado != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoCompletado, 1f);
            Debug.Log("🔊 Reproduciendo sonido de completado");
            yield return new WaitForSecondsRealtime(sonidoCompletado.length);
        }
        else
        {
            Debug.LogWarning("No hay sonido asignado, esperando 0.5 segundos");
            yield return new WaitForSecondsRealtime(0.5f);
        }

        CerrarCanvas();
    }

    void CerrarCanvas()
    {
        Debug.Log("Cerrando canvas...");

        if (canvasMinijuego != null)
        {
            canvasMinijuego.SetActive(false);
        }
        if (tareaManager != null)
            tareaManager.DesbloquearMapaPorTarea();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}