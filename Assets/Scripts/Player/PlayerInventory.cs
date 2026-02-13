using PurrNet;
using UnityEngine;
using UnityEngine.Accessibility;

public class PlayerInventory : NetworkBehaviour
{
    public static PlayerInventory localInventory;

    [SerializeField] private KeyCode useItemKey, consumeItemKey;
    [SerializeField] private Transform itemPoint;
    private Item _itemInHand;

    private const int ignoreRaycastLayer = 2;
    protected override void OnSpawned()
    {
        base.OnSpawned();

        if(!isOwner)
            return;

        localInventory = this;
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();

        if(!isOwner)
            return;

        localInventory = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(useItemKey))
            UseItem();
        if (Input.GetKeyDown(consumeItemKey))
            ConsumeItem();
    }

    private void ConsumeItem()
    {
        if(!_itemInHand)
            return;
        _itemInHand.ConsumeItem();
    }

    private void UseItem()
    {
        if (!_itemInHand)
            return;
        _itemInHand.UseItem();
    }
    
    public void EquipItem(Item item)
    {
        if (!item)
            return;
            
        _itemInHand = Instantiate(item, itemPoint.position, itemPoint.rotation, itemPoint);
        _itemInHand.SetKinematic(true);
        _itemInHand.QueueOnSpawned(() => _itemInHand.SetLayer(ignoreRaycastLayer));
    }

    public void UnequipItem(Item item)
    {
        if (!item)
            return; 
        
        if(!_itemInHand)
            return;
        
        if(_itemInHand.ItemName != item.ItemName)
            return; 
        
        Destroy(_itemInHand.gameObject);
        _itemInHand = null;
    }
    
    public bool isHoldingItem(Item item)
    {
        if(!_itemInHand)
            return false;
        
        return item == _itemInHand;
    }
}
