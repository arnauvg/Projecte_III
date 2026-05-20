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

    [Header("Máquina de escribir")]
    [SerializeField] private float charDelay = 0.05f;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioSource audioSource;

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

    // 🟢 Usamos Awake para ocultar la UI antes del primer frame
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
            {
                dialogueContainer.style.display = DisplayStyle.None;
            }
        }

        // Configurar audio
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null && typingSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        // Doble seguridad: si por algún motivo no se ocultó en Awake, lo ocultamos ahora
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

    public void StartDialogue(int dialogueIndex)
    {
        if (dialogueIndex < 0 || dialogueIndex >= dialogues.Length)
        {
            Debug.LogError("Índice de diálogo inválido: " + dialogueIndex);
            return;
        }

        currentDialogue = dialogueIndex;
        currentSentence = 0;
        isActive = true;
        dialogueContainer.style.display = DisplayStyle.Flex;
        ShowCurrentSentence();
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
        if (nextIndicator != null) nextIndicator.style.display = DisplayStyle.None;

        // Detener cualquier sonido residual antes de empezar a escribir
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

        // 🟢 Al terminar, paramos el audio por si queda algún clip largo (no suele pasar, pero por seguridad)
        if (audioSource != null) audioSource.Stop();

        isTyping = false;
        if (nextIndicator != null)
        {
            nextIndicator.style.display = DisplayStyle.Flex;
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkIndicator());
        }
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        textLabel.text = fullText;
        isTyping = false;

        // Detener el sonido inmediatamente al saltar
        if (audioSource != null) audioSource.Stop();

        if (nextIndicator != null)
        {
            nextIndicator.style.display = DisplayStyle.Flex;
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkIndicator());
        }
    }

    private IEnumerator BlinkIndicator()
    {
        if (nextIndicator == null) yield break;
        nextIndicator.style.opacity = 1f;
        bool visible = true;
        float blinkSpeed = 0.5f;

        while (!isTyping && isActive)
        {
            visible = !visible;
            nextIndicator.style.opacity = visible ? 1f : 0.3f;
            yield return new WaitForSeconds(blinkSpeed);
        }
        nextIndicator.style.opacity = 1f;
    }

    private void Advance()
    {
        if (!isActive || isTyping) return;

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
        if (nextIndicator != null) nextIndicator.style.display = DisplayStyle.None;
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (audioSource != null) audioSource.Stop(); // Aseguramos que el sonido se detiene al acabar el diálogo
    }

    public void ForceEndDialogue()
    {
        if (isActive)
        {
            // Detener corrutinas de escritura y parpadeo
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);

            // Detener sonido de typing
            if (audioSource != null) audioSource.Stop();

            // Ocultar UI
            if (dialogueContainer != null)
                dialogueContainer.style.display = DisplayStyle.None;
            if (nextIndicator != null)
                nextIndicator.style.display = DisplayStyle.None;

            isActive = false;
            isTyping = false;
            Debug.Log("[Cheat] Diálogo forzado a terminar.");
        }
        else
        {
            // Por si acaso la UI quedó visible por error
            if (dialogueContainer != null && dialogueContainer.style.display != DisplayStyle.None)
                dialogueContainer.style.display = DisplayStyle.None;
        }
    }
}