using PurrNet;
using UnityEngine;
public class WorldResources : NetworkBehaviour, IsDamagable
{
    [SerializeField] private SyncVar<int> resourceHealth = new(20);

    public void TakeDamage(int damage)
    {
        TakeDamage_Server(damage);
    }

    [ServerRpc(requireOwnership: false)]
    private void TakeDamage_Server(int damageToTake)
    {
        resourceHealth.value -= damageToTake;
        if(resourceHealth <= 0)
            Die();
    }
    
    public void Die()
    {
        Destroy(gameObject);
    }
}
