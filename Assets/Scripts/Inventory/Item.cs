using PurrNet;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemPicture;

    public string ItemName => itemName;
    public Sprite ItemPicture => itemPicture;

    [ContextMenu("Test Pickup")]
    public void Pickup()
    {
        if (InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
        {
            Debug.LogError($"Couldnt get inventory manager for item{itemName}!", this);
        }
            
        inventoryManager.AddItem(this);
        Destroy(gameObject);
    }
}
