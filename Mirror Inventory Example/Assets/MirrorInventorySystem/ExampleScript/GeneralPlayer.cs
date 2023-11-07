using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GeneralPlayer : NetworkBehaviour
{
    public GameObject ClientSidedObjects;
    void Start()
    {
        ClientSidedObjects.SetActive(isLocalPlayer);
    }
}
