using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ManageInventoryExample : MirrorInventory
{
    public EventSystem EventSystem;
    public GraphicRaycaster Raycaster;
    PointerEventData m_PointerEventData;
    GameObject ActiveBobble;
    void Start()
    {
        container = GameObject.Find("Container").GetComponent<ContainerInventory>();
        EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        if (isLocalPlayer)
        {
            StartInitialize(container);
        }
    }

    void Update()
    {
        if (!isLocalPlayer) { return; }
        if (Input.GetButtonUp("Fire1"))
        {
            UploadInventory();
        }
        m_PointerEventData = new PointerEventData(EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        Raycaster.Raycast(m_PointerEventData, results);
        if (results.Count == 0) { return; }
        foreach (var result in results)
        {
            if (!result.gameObject.CompareTag("Bobble")) { return; }
            ActiveBobble = result.gameObject;
            if (Input.GetButtonDown("Fire1"))
            {
                if (ReturnNearestSlotNumber(false) >= 0 && ActiveBobble.transform.parent.parent.gameObject == ContainerInventory.gameObject)
                {
                    ActiveBobble.transform.SetParent(ItemsSlots[ReturnNearestSlotNumber(false)].transform);
                    return;
                }
                if (ReturnNearestSlotNumber(true) >= 0 && ActiveBobble.transform.parent.parent.gameObject == PlayerInventory.gameObject)
                {
                    ActiveBobble.transform.SetParent(ContainerSlots[ReturnNearestSlotNumber(true)].transform);
                }
            }
        }
    }
}
