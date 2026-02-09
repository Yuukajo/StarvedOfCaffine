using System;
using System.Collections.Generic;
using PurrNet;
using PurrNet.Utils;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private List<InventorySlot> slots;

   [PurrReadOnly, SerializeField] private InventoryItemData[] _inventoryData;

    public void Awake()
    {
        InstanceHandler.RegisterInstance(this);
        _inventoryData = new InventoryItemData[slots.Count];
        ToggleInventory(false);
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<InventoryManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown (KeyCode.E))
        {
            bool isOpen = canvasGroup.alpha > 0;
            ToggleInventory(!isOpen);
           
        }
    }

    private void ToggleInventory (bool toggle)
    {
        canvasGroup.alpha = toggle ? 1 : 0;
        canvasGroup.blocksRaycasts = !toggle;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = toggle;
    }
    public void AddItem(Item item)
    {
        if(!TryStackItem(item))
        AddNewItem(item);
    }

    private bool TryStackItem(Item item)
    {
        
        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (string.IsNullOrEmpty(data.itemName))
                continue;

            if (data.itemName != item.ItemName)
                continue;

            data.amout++;
            data.inventoryItem.Init(item.ItemName, item.ItemPicture, data.amout);
            _inventoryData[i] = data;

            return true;
        }

        return false;
    }

    private void AddNewItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            if (!slot.isEmpty)
                continue;

            var inventoryItem = Instantiate(itemPrefab, slot.transform);
            inventoryItem.Init(item.ItemName, item.ItemPicture, 1);

            var itemData = new InventoryItemData()
            {
                itemName = item.ItemName,
                inventoryItem = inventoryItem,
                amout = 1
            };


            _inventoryData[i] = itemData;
            slot.SetItem(inventoryItem);
            break;

        }
    }


    public void ItemMoved(InventoryItem item, InventorySlot newSlot)
    {
        var newSlotIndex = slots.IndexOf(newSlot);
        var oldSlotIndex = Array.FindIndex(_inventoryData, x => x.inventoryItem == item);
        if (oldSlotIndex == -1)
        {
            Debug.LogError($"Couldnt find item {item.name} in inv data", this);
            return;
        }

        var oldData = _inventoryData[oldSlotIndex];

        _inventoryData[oldSlotIndex] = default;
        _inventoryData[newSlotIndex] = oldData;
    }

    [Serializable]
    public struct InventoryItemData
    {
        public string itemName;
        public InventoryItem inventoryItem;
        public int amout;
        
    }
}
