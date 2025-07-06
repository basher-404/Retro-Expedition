using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Dialogue : MonoBehaviour
{
    //UI References
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image avatarImage;
    [SerializeField] Button talk_Button;
    [SerializeField] Button nextSentenceButton;

    //Dialogue Content
    [SerializeField] private string[] speaker;
    [SerializeField] [TextArea] private string[] dialogueWords;
    [SerializeField] private Sprite [] avatar;
    private bool canStartDialogue;
    private int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canStartDialogue== true)
        {
            startDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && currentIndex < speaker.Length)
        {
            talk_Button.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        talk_Button.gameObject.SetActive(false);
        dialogueCanvas.SetActive(false);
    }

    public void startDialogue()
    {
          if(currentIndex >= speaker.Length)
          {
                dialogueCanvas.SetActive (false);
                return;
          }

          dialogueCanvas.SetActive(true);
          speakerText.text = speaker[currentIndex];
          dialogueText.text = dialogueWords[currentIndex];
          avatarImage.sprite = avatar[currentIndex];
        
    }

    public void button_pressed()
    {
        canStartDialogue = true;
        talk_Button.gameObject.SetActive(false); 
    }

    public void incrementIndex()
    {
        currentIndex++;
    }
}
