using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GeneralPlayerExample : NetworkBehaviour
{
    public GameObject ClientSidedObjects;
    void Start()
    {
        ClientSidedObjects.SetActive(isLocalPlayer);
    }
}
