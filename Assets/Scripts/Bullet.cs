using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    private Sprite _defaultSprite;
    public int damage = 50;
    public float lifetime = 2.0f;

    public Sprite muzzleFlash;

    public int FramesToFlash = 3;
    private SpriteRenderer spriteRend;

    void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        _defaultSprite = spriteRend.sprite;
        StartCoroutine(FlashMuzzleFlash());
        Destroy(gameObject, lifetime);
    }

    IEnumerator FlashMuzzleFlash()
    {
        spriteRend.sprite = muzzleFlash;
        for (int i = 0; i < FramesToFlash; i++)
        {
            yield return 0;
        }

        spriteRend.sprite = _defaultSprite;
    }
}