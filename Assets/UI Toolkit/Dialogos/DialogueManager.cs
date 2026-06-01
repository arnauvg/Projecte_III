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
    [SerializeField] private float charDelay = 1f;
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

        // Iniciar parpadeo del indicador (estará siempre visible)
        if (nextIndicator != null && blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkIndicator());

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
        // Ya no ocultamos el indicador
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
        // No hacemos nada con el indicador, sigue parpadeando
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

        while (isActive) // Parpadea mientras el diálogo esté activo, aunque se esté escribiendo
        {
            visible = !visible;
            nextIndicator.style.opacity = visible ? 1f : 0.4f;
            yield return new WaitForSeconds(blinkSpeed);
        }
        // Al salir, restaurar opacidad total
        nextIndicator.style.opacity = 1f;
    }

    private void Advance()
    {
        if (!isActive) return; // No se avanza si el diálogo no está activo

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
    }

    public void ForceEndDialogue()
    {
        if (isActive) EndDialogue();
        else if (dialogueContainer != null) dialogueContainer.style.display = DisplayStyle.None;
    }
}