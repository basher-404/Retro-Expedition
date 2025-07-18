using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_7_Event_Manager : MonoBehaviour
{
    public Dialogue crabbyDialogue1; // Assign Crabby's Dialogue component
    public GameObject trampoline;   // Assign the trampoline GameObject

    private void Start()
    {
        // Ensure trampoline is initially inactive
        if (trampoline != null)
            trampoline.SetActive(false);
    }

    private void Update()
    {
        // Check if Crabby dialogue is completed
        if (crabbyDialogue1 != null && crabbyDialogue1.dialogueCompleted)
        {
            ActivateTrampoline();
        }
    }

    private void ActivateTrampoline()
    {
        if (trampoline != null)
        {
            trampoline.SetActive(true);
            // Disable this script after activation
            enabled = false;
        }
    }
}
