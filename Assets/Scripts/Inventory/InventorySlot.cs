using PurrNet;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private InventoryItem _item;
    public bool isEmpty => _item == null;
    public InventoryItem Item => _item;

    public void SetItem(InventoryItem item)
    {
        _item = item;
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        RectTransform item =
            eventData.pointerDrag.GetComponent<RectTransform>();

        item.SetParent(transform);
        item.anchoredPosition = Vector2.zero;

        if(!InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
        {
            Debug.LogError($"Failed to get inv manager!", this);
            return;
        }

        InventoryItem inventoryItem =
    eventData.pointerDrag.GetComponent<InventoryItem>();

        inventoryManager.ItemMoved(inventoryItem, this);

        if (inventoryItem == null)
            return;

        inventoryManager.ItemMoved(inventoryItem, this);
    }
}
