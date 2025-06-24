using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Cooldown : MonoBehaviour
{
    private float cooldownTime = 0.2f;

    // Allow dynamic duration setting
    public void SetDuration(float duration)
    {
        cooldownTime = duration;
    }

    private void Start()
    {
        Destroy(this, cooldownTime);
    }
}
