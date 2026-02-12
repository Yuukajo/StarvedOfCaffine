using PurrNet;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public static PlayerInventory localInventory;

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

    public void EquipItem(Item item)
    {
        
    }
}
