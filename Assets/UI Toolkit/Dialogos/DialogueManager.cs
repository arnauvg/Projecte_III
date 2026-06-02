using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class DialogueEntry
{
    public string speakerName;
    [TextArea(2, 5)] public List<string> sentences;
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Diálogos")]
    [SerializeField] private DialogueEntry[] dialogues;

    [Header("Typewriter")]
    [SerializeField] private float charDelay = 0.05f;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioSource audioSource;

    // Eventos
    public static System.Action OnTutorialEnded;
    public static System.Action OnJefeEnded;
    public static System.Action OnVisitanteEnded;

    private VisualElement dialogueContainer;
    private Label speakerLabel;
    private Label textLabel;
    private Label nextIndicator;
    private bool isActive = false;
    private int currentDialogue = 0;
    private int currentSentence = 0;
    private bool isTyping = false;
    private string fullText;
    private Coroutine typingCoroutine;
    private Coroutine blinkCoroutine;
    private bool esDialogoVisitante = false;

    private void Awake()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            var root = uiDocument.rootVisualElement;
            dialogueContainer = root.Q<VisualElement>("DialogueContainer");
            speakerLabel = root.Q<Label>("SpeakerNameLabel");
            textLabel = root.Q<Label>("DialogueTextLabel");
            nextIndicator = root.Q<Label>("NextIndicatorLabel");

            if (dialogueContainer != null)
                dialogueContainer.style.display = DisplayStyle.None;
        }

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null && typingSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        if (dialogueContainer != null && dialogueContainer.style.display != DisplayStyle.None)
            dialogueContainer.style.display = DisplayStyle.None;
    }

    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
                SkipTyping();
            else
                Advance();
        }
    }

    public void StartDialogue(int dialogueIndex, bool esVisitante = false)
    {
        if (dialogueIndex < 0 || dialogueIndex >= dialogues.Length)
        {
            Debug.LogError("Índice de diálogo inválido: " + dialogueIndex);
            return;
        }

        esDialogoVisitante = esVisitante;
        currentDialogue = dialogueIndex;
        currentSentence = 0;
        isActive = true;
        dialogueContainer.style.display = DisplayStyle.Flex;

        if (nextIndicator != null && blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkIndicator());

        ShowCurrentSentence();

        if (!esVisitante && PausaManager.Instance != null)
            PausaManager.Instance.PausarJuego();
    }

    public void CerrarDialogoVisitante()
    {
        if (isActive && esDialogoVisitante)
        {
            Debug.Log("🔇 Cerrando diálogo de visitante");
            EndDialogue();
        }
    }

    private void ShowCurrentSentence()
    {
        var entry = dialogues[currentDialogue];
        speakerLabel.text = entry.speakerName;
        fullText = entry.sentences[currentSentence];
        textLabel.text = "";

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(Typewriter());
    }

    private IEnumerator Typewriter()
    {
        isTyping = true;
        if (audioSource != null) audioSource.Stop();

        for (int i = 0; i <= fullText.Length; i++)
        {
            textLabel.text = fullText.Substring(0, i);
            if (i < fullText.Length && !char.IsWhiteSpace(fullText[i]) && typingSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(typingSound);
            }
            yield return new WaitForSeconds(charDelay);
        }

        if (audioSource != null) audioSource.Stop();
        isTyping = false;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        textLabel.text = fullText;
        isTyping = false;
        if (audioSource != null) audioSource.Stop();
    }

    private IEnumerator BlinkIndicator()
    {
        if (nextIndicator == null) yield break;
        nextIndicator.style.opacity = 1f;
        bool visible = true;
        float blinkSpeed = 0.5f;

        while (isActive)
        {
            visible = !visible;
            nextIndicator.style.opacity = visible ? 1f : 0.4f;
            yield return new WaitForSeconds(blinkSpeed);
        }
        nextIndicator.style.opacity = 1f;
    }

    private void Advance()
    {
        if (!isActive) return;

        var entry = dialogues[currentDialogue];
        currentSentence++;

        if (currentSentence < entry.sentences.Count)
        {
            ShowCurrentSentence();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isActive = false;
        dialogueContainer.style.display = DisplayStyle.None;
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (audioSource != null) audioSource.Stop();
        blinkCoroutine = null;

        if (!esDialogoVisitante)
        {
            Telefono telefono = FindFirstObjectByType<Telefono>();
            if (telefono != null) telefono.OnDialogoTerminado();

            if (currentDialogue == 0)
            {
                OnTutorialEnded?.Invoke();
                Debug.Log("📢 Evento OnTutorialEnded disparado");
            }
            else if (currentDialogue == 1)
            {
                OnJefeEnded?.Invoke();
                Debug.Log("📢 Evento OnJefeEnded disparado");
            }

            if (PausaManager.Instance != null)
                PausaManager.Instance.ReanudarJuego();
        }
        else
        {
            OnVisitanteEnded?.Invoke();
            Debug.Log("📢 Evento OnVisitanteEnded disparado");
        }

        Debug.Log($"📞 Diálogo terminado (tipo: {(esDialogoVisitante ? "visitante" : "sistema")})");
    }

    public void ForceEndDialogue()
    {
        if (isActive) EndDialogue();
        else if (dialogueContainer != null) dialogueContainer.style.display = DisplayStyle.None;
    }

    public void MostrarDialogoVisitante(string nombre, string mensaje)
    {
        DialogueEntry entry = new DialogueEntry();
        entry.speakerName = nombre;
        entry.sentences = new List<string>();
        entry.sentences.Add(mensaje);

        DialogueEntry[] tempDialogues = dialogues;
        dialogues = new DialogueEntry[] { entry };

        StartDialogue(0, true);
        StartCoroutine(RestaurarDialogos(tempDialogues, entry.sentences[0].Length * charDelay + 1.5f));
    }

    IEnumerator RestaurarDialogos(DialogueEntry[] original, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        dialogues = original;
    }
}