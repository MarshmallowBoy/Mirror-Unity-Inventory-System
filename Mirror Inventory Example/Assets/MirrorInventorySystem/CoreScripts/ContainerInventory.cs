using UnityEngine;
using Mirror;

public struct item
{
    public int id;
    public int value;
}

[RequireComponent(typeof(NetworkIdentity))]
public class ContainerInventory : NetworkBehaviour
{
    public readonly SyncList<item> inventory = new SyncList<item>();

    public GameObject[] ItemsToLoad;
}