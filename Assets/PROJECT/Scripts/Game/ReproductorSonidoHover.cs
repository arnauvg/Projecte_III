using UnityEngine;

public class ReproductorSonidoHover : MonoBehaviour
{
    [Header("Sonido")]
    public AudioClip sonidoHover; // Ahora es público y visible en el Inspector
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void ReproducirHover()
    {
        if (sonidoHover != null && audioSource != null)
            audioSource.PlayOneShot(sonidoHover);
    }
}