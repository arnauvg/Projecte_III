using UnityEngine;

public class Telefono : Interactuable
{
    [Header("Audio")]
    public AudioClip ringtone;
    public AudioSource audioSource;

    [Header("Diálogos")]
    public DialogueManager dialogueManager;
    public int dialogoTutorialIndex = 0;
    public int dialogoJefeIndex = 1;

    private bool enMano = false;
    private static bool tutorialCompletado = false;
    private static bool segundaLLamadaPendiente = false;
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
        rb.isKinematic = true;

        GameObject punto = new GameObject("PuntoTelefono");
        punto.transform.SetParent(Camera.main.transform);
        punto.transform.localPosition = new Vector3(-0.35f, -0.1f, 0.5f);
        punto.transform.localRotation = Quaternion.Euler(15f, 30f, 0f);
        puntoMano = punto.transform;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;

        if (dialogueManager == null) dialogueManager = FindFirstObjectByType<DialogueManager>();

        TareaManager.OnPrimeraTareaActivada += SegundaLlamada;

        if (!tutorialCompletado && ringtone != null)
        {
            audioSource.clip = ringtone;
            audioSource.Play();
            if (PausaManager.Instance != null) PausaManager.Instance.PausarJuego();
            Debug.Log("📞 Primera llamada (tutorial) - Teléfono sonando");
        }
    }

    void OnDestroy()
    {
        TareaManager.OnPrimeraTareaActivada -= SegundaLlamada;
    }

    void SegundaLlamada()
    {
        if (segundaLLamadaPendiente) return;
        if (tutorialCompletado == false) return;

        // 🔥 Cerrar cualquier diálogo de visitante activo
        if (dialogueManager != null)
        {
            dialogueManager.CerrarDialogoVisitante();
        }

        segundaLLamadaPendiente = true;

        Debug.Log("📞 Segunda llamada: El jefe explica las tareas");

        if (ringtone != null && audioSource != null)
        {
            audioSource.clip = ringtone;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (PausaManager.Instance != null)
            PausaManager.Instance.PausarJuego();
    }

    void OnMouseDown()
    {
        Debug.Log("🖱️ OnMouseDown detectado en teléfono");
        Recoger();
    }

    public override bool Recoger()
    {
        Debug.Log($"📞 Recogiendo teléfono - enMano={enMano}, dialogoEnProgreso={dialogoEnProgreso}, tutorialCompletado={tutorialCompletado}, segundaLLamadaPendiente={segundaLLamadaPendiente}");

        if (!enMano && !dialogoEnProgreso)
        {
            enMano = true;
            rb.isKinematic = true;
            rb.useGravity = false;

            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.loop = false;

            if (dialogueManager != null)
            {
                dialogoEnProgreso = true;

                if (!tutorialCompletado)
                {
                    Debug.Log("📞 Iniciando diálogo del tutorial");
                    dialogueManager.StartDialogue(dialogoTutorialIndex, false);
                    tutorialCompletado = true;
                }
                else if (segundaLLamadaPendiente)
                {
                    Debug.Log("📞 Iniciando diálogo del jefe (tareas)");
                    dialogueManager.StartDialogue(dialogoJefeIndex, false);
                    segundaLLamadaPendiente = false;
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
            rb.isKinematic = true;
            rb.useGravity = false;
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

    public void OnDialogoTerminado()
    {
        dialogoEnProgreso = false;
        enMano = false;
        transform.position = posOriginal;
        transform.rotation = rotOriginal;
        Debug.Log("📞 Diálogo terminado - Teléfono listo");
    }

    public void SkipPhone()
    {
        if (!tutorialCompletado)
        {
            if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
            tutorialCompletado = true;
            if (PausaManager.Instance != null) PausaManager.Instance.ReanudarJuego();
            OnDialogoTerminado();
            Debug.Log("[Cheat] Tutorial saltado");
        }
        else if (segundaLLamadaPendiente)
        {
            if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
            segundaLLamadaPendiente = false;
            if (PausaManager.Instance != null) PausaManager.Instance.ReanudarJuego();
            OnDialogoTerminado();
            Debug.Log("[Cheat] Segunda llamada saltada");
        }
    }

    public static void Resetear()
    {
        tutorialCompletado = false;
        segundaLLamadaPendiente = false;
        Debug.Log("Teléfono reseteado para nueva partida");
    }
}