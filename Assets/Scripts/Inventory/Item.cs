using PurrNet;
using UnityEngine;

public abstract class Item : AInteractable
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemPicture;
    [SerializeField] private Rigidbody rigidbody;

    public string ItemName => itemName;
    public Sprite ItemPicture => itemPicture;

    protected override void OnOwnerChanged(PlayerID? oldOwner, PlayerID? newOwner, bool asServer)
    {
        base.OnOwnerChanged(oldOwner, newOwner, asServer);

        if (PlayerInventory.localInventory.isHoldingItem(this))
        {
            rigidbody.isKinematic = true;
            return;
        }
        rigidbody.isKinematic = !isOwner;
    }

    [ContextMenu("Test Pickup")]
    public void Pickup()
    {
        if (!InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
        {
            Debug.LogError($"Couldnt get inventory manager for item{itemName}!", this);
            return;
        }
            
        inventoryManager.AddItem(this);
        Destroy(gameObject);
    }

    public override void Interact()
    {
        Pickup(); 
    }

    public virtual void UseItem()
    {
        
    }

    public virtual void ConsumeItem()
    {
        
    }
    
    public void SetKinematic(bool toggle)
    {
        rigidbody.isKinematic = toggle;
    }

    [ObserversRpc(bufferLast:true)]

    public void SetLayer(int ignoreRaycastLayer)
    {
        SetLayerRecursive(gameObject, ignoreRaycastLayer);
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);
    }
}
