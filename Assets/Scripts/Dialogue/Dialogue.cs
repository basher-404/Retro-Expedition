using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Dialogue : MonoBehaviour
{
    // UI References
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Button talk_Button;
    [SerializeField] private Button nextSentenceButton;

    // Animation Targets
    [SerializeField] private RectTransform namePanel;
    [SerializeField] private RectTransform dialoguePanel;
    [SerializeField] private RectTransform avatarTransform;

    // Dialogue Content
    [SerializeField] private string[] speaker;
    [SerializeField][TextArea] private string[] dialogueWords;
    [SerializeField] private Sprite[] avatar;

    // Animation Settings
    [SerializeField] private float panelAnimationDuration = 0.5f;
    [SerializeField] private float avatarAnimationDuration = 0.7f;
    [SerializeField] private float typewriterSpeed = 0.05f;

    // Dialogue State
    private bool canStartDialogue;
    private int currentIndex;
    private bool isTyping;
    private Coroutine typingRoutine;
    private bool dialogueCompleted;
    private Vector2 namePanelStartPos;
    private Vector2 dialoguePanelStartPos;
    private Vector2 avatarStartPos;
    private bool isFirstDialogue = true;
    private bool isAnimating; // Track if UI animations are playing

    void Start()
    {
        talk_Button.gameObject.SetActive(false);
        dialogueCanvas.SetActive(false);
        dialogueCompleted = false;

        // Store initial positions for animation
        namePanelStartPos = namePanel.anchoredPosition;
        dialoguePanelStartPos = dialoguePanel.anchoredPosition;
        avatarStartPos = avatarTransform.anchoredPosition;

        // Disable next sentence button initially
        nextSentenceButton.interactable = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dialogueCompleted)
        {
            talk_Button.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        talk_Button.gameObject.SetActive(false);
        dialogueCanvas.SetActive(false);
        ResetUIState();
    }

    public void button_pressed()
    {
        canStartDialogue = true;
        talk_Button.gameObject.SetActive(false);
        startDialogue();
    }

    private void startDialogue()
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
        // Disable next button during animations
        nextSentenceButton.interactable = false;
        isAnimating = true;

        ResetUIState();

        namePanel.anchoredPosition = namePanelStartPos - new Vector2(0, 200);
        dialoguePanel.anchoredPosition = dialoguePanelStartPos - new Vector2(0, 200);
        avatarTransform.anchoredPosition = avatarStartPos - new Vector2(300, 0);

        // Animate panels rising
        yield return StartCoroutine(AnimatePanel(namePanel, namePanelStartPos));
        yield return StartCoroutine(AnimatePanel(dialoguePanel, dialoguePanelStartPos));

        // Animate avatar sliding
        yield return StartCoroutine(AnimateAvatar());

        // Re-enable next button after animations
        nextSentenceButton.interactable = true;
        isAnimating = false;

        // Start text animation
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
        // Don't process taps during UI animations
        if (isAnimating) return;

        if (isTyping)
        {
            // Skip text animation
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
            }
            else
            {
                startDialogue();
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
