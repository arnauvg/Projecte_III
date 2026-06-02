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
    private bool colgarAutomaticamente = false;
    private bool esperandoColgar = false; // 🔥 Evita colgar inmediatamente después de recoger

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
        Debug.Log($"🖱️ OnMouseDown - enMano={enMano}, dialogoEnProgreso={dialogoEnProgreso}");

        if (enMano)
        {
            Soltar();
        }
        else
        {
            Recoger();
        }
    }

    public override bool Recoger()
    {
        if (enMano)
        {
            Debug.Log("📞 Ya tienes el teléfono en la mano");
            return false;
        }

        enMano = true;
        rb.isKinematic = true;
        rb.useGravity = false;

        // 🔥 Evitar que se cuelgue inmediatamente
        esperandoColgar = true;
        Invoke(nameof(ResetEsperandoColgar), 0.2f);

        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.loop = false;

        if (dialogueManager != null && !dialogoEnProgreso)
        {
            if (!tutorialCompletado)
            {
                Debug.Log("📞 Iniciando diálogo del tutorial");
                dialogueManager.StartDialogue(dialogoTutorialIndex, false);
                tutorialCompletado = true;
                colgarAutomaticamente = true;
                dialogoEnProgreso = true;
            }
            else if (segundaLLamadaPendiente)
            {
                Debug.Log("📞 Iniciando diálogo del jefe");
                dialogueManager.StartDialogue(dialogoJefeIndex, false);
                segundaLLamadaPendiente = false;
                colgarAutomaticamente = true;
                dialogoEnProgreso = true;
            }
            else
            {
                Debug.Log("📞 Cogiste el teléfono sin motivo (no hay diálogo)");
                colgarAutomaticamente = false;
            }
        }
        else
        {
            colgarAutomaticamente = false;
        }

        return true;
    }

    void ResetEsperandoColgar()
    {
        esperandoColgar = false;
    }

    public override void Soltar()
    {
        if (!enMano) return;

        transform.position = posOriginal;
        transform.rotation = rotOriginal;
        enMano = false;
        rb.isKinematic = true;
        rb.useGravity = false;

        dialogoEnProgreso = false;
        colgarAutomaticamente = false;
        esperandoColgar = false;

        Debug.Log("📞 Teléfono colgado");
    }

    void Update()
    {
        if (enMano)
        {
            transform.position = Vector3.Lerp(transform.position, puntoMano.position, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, puntoMano.rotation, Time.deltaTime * 15f);
        }

        // 🔥 Solo colgar si NO estamos esperando (evita colgar justo después de recoger)
        if (Input.GetMouseButtonDown(0) && enMano && !esperandoColgar)
        {
            Debug.Log("📞 Colgando por clic izquierdo");
            Soltar();
        }

        if (Input.GetKeyDown(KeyCode.R) && enMano)
        {
            Debug.Log("📞 Colgando manualmente (tecla R)");
            Soltar();
        }
    }

    public void OnDialogoTerminado()
    {
        dialogoEnProgreso = false;

        if (colgarAutomaticamente)
        {
            Debug.Log("📞 Diálogo terminado - Colgando automáticamente");
            Soltar();
        }
        else
        {
            Debug.Log("📞 Diálogo terminado - Teléfono en mano, haz clic para colgar");
        }
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