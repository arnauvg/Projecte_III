using UnityEngine;

public class Telefono : Interactuable
{
    [Header("Audio")]
    public AudioClip ringtone;        // Sonido que suena al inicio
    public AudioSource audioSource;   // Se asignará automáticamente si no está

    [Header("Diálogo")]
    public DialogueManager dialogueManager; // Referencia al sistema de diálogo
    public int dialogueIndex = 0;           // Índice del diálogo del jefe

    private bool enMano = false;
    private Vector3 posOriginal;
    private Quaternion rotOriginal;
    private Transform puntoMano;
    private Rigidbody rb;

    void Start()
    {
        posOriginal = transform.position;
        rotOriginal = transform.rotation;
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        // Crear punto de agarre (hijo de la cámara)
        GameObject punto = new GameObject("PuntoTelefono");
        punto.transform.SetParent(Camera.main.transform);
        punto.transform.localPosition = new Vector3(-0.35f, -0.1f, 0.5f);
        punto.transform.localRotation = Quaternion.Euler(15f, 30f, 0f);
        puntoMano = punto.transform;

        // Configurar AudioSource si no está
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;  // Que suene en bucle hasta que lo cojas

        // Reproducir tono al empezar
        if (ringtone != null)
        {
            audioSource.clip = ringtone;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Teléfono sin sonido asignado.");
        }

        // Buscar automáticamente el DialogueManager si no está asignado
        if (dialogueManager == null)
            dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public override bool Recoger()
    {
        if (!enMano)
        {
            enMano = true;
            rb.isKinematic = true;
            rb.useGravity = false;

            // Parar el sonido
            if (audioSource.isPlaying) audioSource.Stop();

            // Iniciar diálogo del jefe
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(dialogueIndex);
            }
            else
            {
                Debug.LogError("No se encontró DialogueManager en la escena.");
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
}