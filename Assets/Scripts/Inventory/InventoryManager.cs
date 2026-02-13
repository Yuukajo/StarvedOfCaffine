using System;
using System.Collections.Generic;
using PurrNet;
using PurrNet.Utils;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> allItems = new();
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private List<InventorySlot> slots = new();
    [SerializeField] private List<ActionSlot> actionSlots = new();
   [PurrReadOnly, SerializeField] private InventoryItemData[] _inventoryData;
   private ActionSlot _activeActionSlot;

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool isOpen = canvasGroup.alpha > 0;
            ToggleInventory(!isOpen);
        }
    }

    private void ToggleInventory (bool toggle)
    {
        canvasGroup.alpha = toggle ? 1 : 0;
        canvasGroup.blocksRaycasts = toggle;
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

            data.amount++;
            data.inventoryItem.Init(item.ItemName, item.ItemPicture, data.amount);
            _inventoryData[i] = data;

            return true;
        }
        return false;
    }

    private void AddNewItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (!slot.isEmpty)
                continue;

            var inventoryItem = Instantiate(itemPrefab, slot.transform);
            inventoryItem.Init(item.ItemName, item.ItemPicture, 1);

            var itemData = new InventoryItemData()
            {
                itemName = item.ItemName,
                itemPicture = item.ItemPicture,
                inventoryItem = inventoryItem,
                amount = 1
            };
            _inventoryData[i] = itemData;
            slot.SetItem(inventoryItem);
            break;
        }
    }
    
    public void DropItem(InventoryItem inventoryItem)
    {
        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (data.inventoryItem != inventoryItem)
                continue;
            
            var itemToSpawn = GetItemByName(data.itemName);
            if (itemToSpawn == null)
            {
                Debug.LogError($"Item to spawn with name {data.itemName} not found! ", this);
                return;
            }
            
            PlayerInventory.localInventory.UnequipItem(itemToSpawn);
            
            Vector3 spawnPosition = PlayerMovement.localPlayerMovement.transform.position + PlayerMovement.localPlayerMovement.transform.forward + Vector3.up;
            var item = Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);

            if(DeductItem(inventoryItem) <= 0)
                PlayerInventory.localInventory.UnequipItem(itemToSpawn);
            break;
        }
    }
    
    private int DeductItem(InventoryItem inventoryItem)
    {
        for (int i = 0; i < _inventoryData.Length; i++)
        {
            var data = _inventoryData[i];
            if (data.inventoryItem != inventoryItem)
                continue;

            data.amount--;
            if (data.amount <= 0)
            {
                _inventoryData[i] = default;
                slots[i].SetItem(null);
                Destroy(inventoryItem.gameObject);
                return 0;
            }
            else
            {
                data.inventoryItem.Init(data.itemName, data.itemPicture, data.amount);
                _inventoryData[i] = data;
                return data.amount;
            }
        }

        return 0;
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

    public void SetActionSlotActive(ActionSlot actionSlot)
    {
        if(_activeActionSlot == actionSlot)
            return;

        if (_activeActionSlot != null)
        {
            PlayerInventory.localInventory.UnequipItem(GetItemByActionSlot(_activeActionSlot));
            _activeActionSlot.ToggleActive(false);
        }
        actionSlot.ToggleActive(true);
        _activeActionSlot = actionSlot;
        
        PlayerInventory.localInventory.EquipItem(GetItemByActionSlot(actionSlot));
    }

    private Item GetItemByName(string itemName)
    {
        return allItems.Find(x => x.ItemName == itemName);
    }

    private Item GetItemByActionSlot(ActionSlot actionSlot)
    {
        var inventorySlot = actionSlot.GetComponent<InventorySlot>();
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if(slots[i] == actionSlot)
                return GetItemByName(_inventoryData[i].itemName);
        }
        return null;
    }
    
    [Serializable]
    public struct InventoryItemData
    {
        public Sprite itemPicture;
        public string itemName;
        public InventoryItem inventoryItem;
        public int amount;
    }
}
