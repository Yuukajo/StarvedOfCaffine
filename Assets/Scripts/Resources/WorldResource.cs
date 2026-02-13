using System;
using System.Collections;
using System.Numerics;
using PurrNet;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class WorldResources : NetworkBehaviour, IsDamagable
{
    [SerializeField] private SyncVar<int> resourceHealth = new(20);
    [SerializeField] private Item dropItem;
    [SerializeField] private Vector2 dropAmountRange = new Vector2(1, 3);
    [SerializeField] private Vector3 dropPosition;
   
    [Header("Animation")]
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float popIntensity = 0.2f;
    [SerializeField] private AnimationCurve popCurve;
    private Coroutine _popCoroutine;

    private void Awake()
    {
        resourceHealth.onChanged += OnHealthChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        resourceHealth.onChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int newHealth)
    {
        if(_popCoroutine != null)
            StopCoroutine(_popCoroutine);
        _popCoroutine = StartCoroutine(DoPop());
    }

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
        var dropAmount = Random.Range(dropAmountRange.x, dropAmountRange.y + 1);
        for (int i = 0; i < dropAmount; i++)
            Instantiate(dropItem, transform.TransformPoint(dropPosition), Quaternion.identity);
        
        Destroy(gameObject);
    }
    
    private IEnumerator DoPop()
    {
        float t = 0;
        Vector3 startScale = transform.localScale;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            
            var popAmount = popCurve.Evaluate(t / popDuration); 
            transform.localScale = startScale + Vector3.one * popAmount;
            
            yield return null;
        }
    }

    private void onDrawGizmos()
    {
        Gizmos.color = Color.blue;
    }
}
