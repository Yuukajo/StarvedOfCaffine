using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PurrNet;
public class Tools : Item
{
    [SerializeField] private int damage = 5;
    [SerializeField] private float useCooldwon = 1f; 
    
    [SerializeField] private Animator animator;

    [SerializeField] private float waitBeforeFirstHit = 0.1f;
    [SerializeField] private float waitBetweenHits = 0.2f;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private List<Vector3> hitPoints = new();
    
    private WaitForSeconds _waitForFirstHit, _waitBetweenHits;
    private float _lastUseTime;

    public void Awake()
    {
        _waitBetweenHits = new WaitForSeconds(waitBetweenHits);
        _waitForFirstHit = new WaitForSeconds(waitBeforeFirstHit);
    }

    public override void UseItem()
    {
        base.UseItem();
        StartCoroutine(HandleHit());
        
        if(_lastUseTime + useCooldwon < Time.time)
            return;
        _lastUseTime = Time.time;
    }

    private Collider[] _colliders = new Collider[15];
    private IEnumerator HandleHit()
    {
        var alreadyHit = new List<IsDamagable>();
        animator.SetTrigger("DoAction");
        yield return _waitForFirstHit;

        foreach (var hitPoint in hitPoints)
        {
            var hitPosition = transform.TransformPoint(hitPoint);
            var hits = Physics.OverlapSphereNonAlloc(hitPosition, hitRadius, _colliders);
            for (int i = 0; i < hits; i++)
            {
                var hit = _colliders[i];
                if(hit.TryGetComponent(out IsDamagable damagable))
                    continue;
                if(alreadyHit.Contains(damagable))
                    continue;
                  
                damagable.TakeDamage(damage);
                alreadyHit.Add(damagable);
            }
            yield return _waitBetweenHits;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var hitPoint in hitPoints)
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(hitPoint), hitRadius);
        }
    }
}
