using Mirror;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class MirrorInventory : NetworkBehaviour
{
    public Transform PlayerInventory;
    public Transform ContainerInventory;
    public GameObject[] ItemsSlots;
    public GameObject[] ContainerSlots;
    public GameObject[] ItemsIndex;
    public ContainerInventory container;
    public item[] Items = new item[20];
    public static item NullItem = new item { id = 0, value = 0 };

    private void Start()
    {
        if (ItemsSlots.Length == 0)
        {
            for (int i = 0; i < ItemsSlots.Length; i++)
            {
                ItemsSlots[i] = PlayerInventory.transform.GetChild(i).gameObject;
            }
        }
        if (ContainerSlots.Length == 0)
        {
            for (int i = 0; i < ContainerSlots.Length; i++)
            {
                ContainerSlots[i] = ContainerInventory.transform.GetChild(i).gameObject;
            }
        }
    }

    public void StartInitialize(ContainerInventory containerInv)
    {
        container = containerInv;
        container.inventory.Callback += LoadInventoryFromContainer;
        LoadInventoryFromContainer(0, 0, NullItem, NullItem);
        if (container.inventory.Count == 0)
        {
            foreach (var items in container.ItemsToLoad)
            {

                if (items == null)
                {
                    InitializeItemCommand(container, NullItem);
                }
                else
                {
                    InventorySystemIdentification ISI = items.GetComponent<InventorySystemIdentification>();
                    InitializeItemCommand(container, new item { id = ISI.id, value = ISI.value });
                }
            }
        }
    }

    void LoadInventoryFromContainer(SyncList<item>.Operation op, int index, item oldItem, item newItem)
    {
        if (container.inventory.Count != container.ItemsToLoad.Length) { return; }
        int i = 0;
        foreach (item ID in container.inventory)
        {
            if (ID.id == 0)
                Items[i].id = 0;
            else
            {
                Items[i].id = ID.id;
                Items[i].value = ID.value;
            }
            i++;
        }
        LoadInventory();
    }

    public void LoadInventory()
    {
        foreach (var containerSlots in ContainerSlots)
        {
            if (containerSlots.transform.childCount > 0)
                DestroyImmediate(containerSlots.transform.GetChild(0).gameObject);
        }
        int i = 0;
        foreach (var Item in Items)
        {
            if (Item.id == 0)
            {
                i++;
                continue;
            }
            GameObject temp = Instantiate(ItemsIndex[Item.id]);
            temp.GetComponent<InventorySystemIdentification>().value = Item.value;
            temp.transform.SetParent(ContainerSlots[i].transform, false);
            i++;
        }
    }

    public void UploadInventory()
    {
        for (int i = 0; i < ContainerSlots.Length; i++)
        {
            if (ContainerSlots[i].transform.childCount == 0)
            {
                Items[i].id = 0;
            }
            else
            {
                Items[i].id = ContainerSlots[i].transform.GetChild(0).GetComponent<InventorySystemIdentification>().id;
                Items[i].value = ContainerSlots[i].transform.GetChild(0).GetComponent<InventorySystemIdentification>().value;
            }
        }
        ClearInvListCommand(container);
        foreach (var Item in Items)
        {
            if (Item.id == 0)
            {
                InitializeItemCommand(container, NullItem);
            }
            else
            {
                InitializeItemCommand(container, Item);
            }
        }
    }

    [Command]
    public void InitializeItemCommand(ContainerInventory containerInv, item id)
    {
        containerInv.inventory.Add(id);
    }

    [Command]
    public void ClearInvListCommand(ContainerInventory containerInv)
    {
        containerInv.inventory.Clear();
    }

    public bool InventoryFull(bool ContainerInventory)
    {
        bool inventoryFull = true;
        if (ContainerInventory)
        {
            foreach (var item in ContainerSlots)
            {
                if (item.GetComponentInChildren<InventorySystemIdentification>() == null)
                {
                    inventoryFull = false;
                    continue;
                }
            }
        }
        else
        {
            foreach (var item in ItemsSlots)
            {
                if (item.GetComponentInChildren<InventorySystemIdentification>() == null)
                {
                    inventoryFull = false;
                    continue;
                }
            }
        }
        return inventoryFull;
    }

    public void AddItemByID(int ID)
    {
        foreach(var item in ItemsSlots)
        {
            if (item.GetComponentInChildren<InventorySystemIdentification>() == null)
            {
                GameObject temp = Instantiate(ItemsIndex[ID]);
                temp.transform.SetParent(item.transform, false);
                return;
            }
        }
    }

    public void DestroyItemByID(int ID)
    {
        foreach (var item in ItemsSlots)
        {
            if (item.GetComponentInChildren<InventorySystemIdentification>() == null)
            {
                continue;
            }
            if (item.GetComponentInChildren<InventorySystemIdentification>().id == ID)
            {
                foreach (Transform child in item.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public int ReturnNearestSlotNumber(bool ContainerInventory)
    {
        if (ContainerInventory)
        {
            for (int i = 0; i < ContainerSlots.Length; i++)
            {
                if (ContainerSlots[i].GetComponentInChildren<InventorySystemIdentification>() == null)
                {
                    return i;
                }
            }
            return -1;
        }
        for (int i = 0; i < ItemsSlots.Length; i++)
        {
            if (ItemsSlots[i].GetComponentInChildren<InventorySystemIdentification>() == null)
            {
                return i;
            }
        }
        return -1;
    }

    public string SaveInventoryToString()
    {
        string listOfItemsToString = string.Empty;
        foreach (var itemSlot in ItemsSlots)
        {
            item Item = NullItem;
            if (itemSlot.GetComponentInChildren<InventorySystemIdentification>() == null)
            {
                Item = NullItem;
            }
            else
            {
                Item = new item
                {
                    id = itemSlot.GetComponentInChildren<InventorySystemIdentification>().id,
                    value = itemSlot.GetComponentInChildren<InventorySystemIdentification>().value
                };
            }
            string itemToString = Item.id.ToString() + "," + Item.value + ":";
            listOfItemsToString += itemToString;
        }
        return listOfItemsToString;
    }

    public void LoadInventoryFromString(string InventoryAsString)
    {
        item Item = new item { };
        string[] itemListString = InventoryAsString.Split(':');
        int i = 0;
        foreach (var itemsSlots in ItemsSlots)
        {
            Item.id = int.Parse(itemListString[i].Split(',')[0]);
            Item.value = int.Parse(itemListString[i].Split(",")[1]);
            if (Item.id == 0)
            {
                if (itemsSlots.transform.childCount > 0)
                {
                    Destroy(itemsSlots.transform.GetChild(0));
                }
                else
                {
                    continue;
                }
            }
            GameObject temp = Instantiate(ItemsIndex[Item.id]);
            temp.GetComponent<InventorySystemIdentification>().value = Item.value;
            temp.transform.SetParent(itemsSlots.transform, false);
            i++;
        }
    }
}