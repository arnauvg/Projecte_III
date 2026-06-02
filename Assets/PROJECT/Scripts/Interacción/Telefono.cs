using UnityEngine;

public class Telefono : Interactuable
{
    public AudioClip ringtone;
    public AudioSource audioSource;
    public DialogueManager dialogueManager;
    public int dialogueIndex = 0;

    private bool enMano = false;
    private static bool yaActivado = false;
    private static bool tutorialCompletado = false; // ← NUEVO
    private Vector3 posOriginal;
    private Quaternion rotOriginal;
    private Transform puntoMano;
    private Rigidbody rb;
    private bool dialogoEnProgreso = false;

    void Start()
    {
        posOriginal = transform.position;
        rotOriginal = transform.rotation;
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        GameObject punto = new GameObject("PuntoTelefono");
        punto.transform.SetParent(Camera.main.transform);
        punto.transform.localPosition = new Vector3(-0.35f, -0.1f, 0.5f);
        punto.transform.localRotation = Quaternion.Euler(15f, 30f, 0f);
        puntoMano = punto.transform;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;

        if (!yaActivado && ringtone != null)
        {
            audioSource.clip = ringtone;
            audioSource.Play();
            if (PausaManager.Instance != null) PausaManager.Instance.PausarJuego();
        }

        if (dialogueManager == null) dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    public override bool Recoger()
    {
        if (!enMano)
        {
            enMano = true;
            rb.isKinematic = true;
            rb.useGravity = false;

            if (!yaActivado && !dialogoEnProgreso)
            {
                if (audioSource.isPlaying) audioSource.Stop();
                if (dialogueManager != null)
                {
                    dialogoEnProgreso = true;
                    dialogueManager.StartDialogue(dialogueIndex);
                    yaActivado = true;
                }
            }
            return true;
        }
        return false;
    }

    public override void Soltar()
    {
        if (enMano)
        {
            transform.position = posOriginal;
            transform.rotation = rotOriginal;
            enMano = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (enMano)
        {
            transform.position = Vector3.Lerp(transform.position, puntoMano.position, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, puntoMano.rotation, Time.deltaTime * 15f);
        }
    }

    public void SkipPhone()
    {
        if (!yaActivado)
        {
            if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
            yaActivado = true;
            if (PausaManager.Instance != null) PausaManager.Instance.ReanudarJuego();
        }
    }

    public static void Resetear()
    {
        yaActivado = false;
        tutorialCompletado = false;
    }

    public static bool EstaActivo()
    {
        return yaActivado;
    }
}