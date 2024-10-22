using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AutoDestroy : MonoBehaviour
{
    public float minLifetime, maxLifetime;
    void Start()
    {
        StartCoroutine(HandleLifetime());
    }
    private IEnumerator HandleLifetime()
    {
        yield return new WaitForSeconds(Random.Range(minLifetime, maxLifetime));
        Destroy(gameObject);
    }
}