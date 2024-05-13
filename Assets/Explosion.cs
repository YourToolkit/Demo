using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Duration = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.Play("Explosion");
        transform.DOScale(Vector3.one * 3, Duration).OnComplete(() =>
        {
            transform.DOScale(Vector3.zero, Duration).SetDelay(Duration).OnComplete(() => { Destroy(gameObject); });
        });
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            other.transform.GetComponent<EnemyController>().Die();
        }
    }
}