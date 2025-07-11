using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public enum DialogueMode
{
    NPC,        // Requires talk button press
    Tutorial    // Starts automatically
}

public class Dialogue : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Button talk_Button;
    [SerializeField] private Button nextSentenceButton;

    [Header("Animation Targets")]
    [SerializeField] private RectTransform namePanel;
    [SerializeField] private RectTransform dialoguePanel;
    [SerializeField] private RectTransform avatarTransform;

    [Header("Dialogue Content")]
    [SerializeField] private string[] speaker;
    [SerializeField][TextArea] private string[] dialogueWords;
    [SerializeField] private Sprite[] avatar;

    [Header("Animation Settings")]
    [SerializeField] private float panelAnimationDuration = 0.5f;
    [SerializeField] private float avatarAnimationDuration = 0.7f;
    [SerializeField] private float typewriterSpeed = 0.05f;

    [Header("Configuration")]
    [SerializeField] private DialogueMode dialogueMode = DialogueMode.NPC;

    // Dialogue State
    private int currentIndex;
    private bool isTyping;
    private Coroutine typingRoutine;
    private bool dialogueCompleted;
    private Vector2 namePanelStartPos;
    private Vector2 dialoguePanelStartPos;
    private Vector2 avatarStartPos;
    private bool isFirstDialogue = true;
    private bool isAnimating;
    private bool playerInTrigger;

    void Start()
    {
        talk_Button.gameObject.SetActive(false);
        dialogueCanvas.SetActive(false);
        dialogueCompleted = false;

        // Store initial positions for animation
        namePanelStartPos = namePanel.anchoredPosition;
        dialoguePanelStartPos = dialoguePanel.anchoredPosition;
        avatarStartPos = avatarTransform.anchoredPosition;

        nextSentenceButton.interactable = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dialogueCompleted)
        {
            playerInTrigger = true;

            if (dialogueMode == DialogueMode.NPC)
            {
                talk_Button.gameObject.SetActive(true);
            }
            else if (dialogueMode == DialogueMode.Tutorial)
            {
                StartDialogue();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
            talk_Button.gameObject.SetActive(false);

            if (dialogueMode == DialogueMode.Tutorial)
            {
                // For tutorials, close dialogue when leaving
                dialogueCanvas.SetActive(false);
                ResetUIState();
            }
        }
    }

    public void ButtonPressed()
    {
        if (dialogueMode == DialogueMode.NPC && playerInTrigger)
        {
            talk_Button.gameObject.SetActive(false);
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (currentIndex >= speaker.Length)
        {
            dialogueCompleted = true;
            dialogueCanvas.SetActive(false);
            return;
        }

        dialogueCanvas.SetActive(true);

        if (isFirstDialogue)
        {
            StartCoroutine(AnimateDialogueUI());
            isFirstDialogue = false;
        }
        else
        {
            UpdateDialogueContent();
        }
    }

    private IEnumerator AnimateDialogueUI()
    {
        nextSentenceButton.interactable = false;
        isAnimating = true;

        ResetUIState();

        namePanel.anchoredPosition = namePanelStartPos - new Vector2(0, 200);
        dialoguePanel.anchoredPosition = dialoguePanelStartPos - new Vector2(0, 200);
        avatarTransform.anchoredPosition = avatarStartPos - new Vector2(300, 0);

        yield return StartCoroutine(AnimatePanel(namePanel, namePanelStartPos));
        yield return StartCoroutine(AnimatePanel(dialoguePanel, dialoguePanelStartPos));
        yield return StartCoroutine(AnimateAvatar());

        nextSentenceButton.interactable = true;
        isAnimating = false;

        UpdateDialogueContent();
    }

    private IEnumerator AnimateAvatar()
    {
        Vector2 targetPos = avatarStartPos;
        Vector2 startPos = avatarTransform.anchoredPosition;
        float elapsed = 0;

        while (elapsed < avatarAnimationDuration)
        {
            avatarTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / avatarAnimationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        avatarTransform.anchoredPosition = targetPos;
    }

    private void UpdateDialogueContent()
    {
        dialogueText.text = "";
        speakerText.text = "";

        speakerText.text = speaker[currentIndex];
        avatarImage.sprite = avatar[currentIndex];

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeText(dialogueWords[currentIndex]));
    }

    private IEnumerator AnimatePanel(RectTransform panel, Vector2 targetPos)
    {
        Vector2 startPos = panel.anchoredPosition;
        float elapsed = 0;

        while (elapsed < panelAnimationDuration)
        {
            panel.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / panelAnimationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = targetPos;
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
    }

    public void OnDialogueTap()
    {
        if (isAnimating) return;

        if (isTyping)
        {
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            dialogueText.text = dialogueWords[currentIndex];
            isTyping = false;
        }
        else
        {
            currentIndex++;

            if (currentIndex >= speaker.Length)
            {
                dialogueCompleted = true;
                dialogueCanvas.SetActive(false);
                isFirstDialogue = true;
                currentIndex = 0;

                // For tutorials, disable after completion
                if (dialogueMode == DialogueMode.Tutorial)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                StartDialogue();
            }
        }
    }

    private void ResetUIState()
    {
        speakerText.text = "";
        dialogueText.text = "";
        avatarTransform.anchoredPosition = avatarStartPos;
        namePanel.anchoredPosition = namePanelStartPos;
        dialoguePanel.anchoredPosition = dialoguePanelStartPos;
    }
}