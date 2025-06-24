using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Manager : MonoBehaviour
{
    private void Start()
    {
        Portal_Node[] portals = GetComponentsInChildren<Portal_Node>();

        if (portals.Length != 2)
        {
            Debug.LogError("Portal system requires exactly 2 PortalNode children");
            return;
        }

        // Pair portals bidirectionally
        portals[0].pairedPortal = portals[1];
        portals[1].pairedPortal = portals[0];
    }
}
