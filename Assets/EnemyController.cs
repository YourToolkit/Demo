using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float Speed = 2.0f;
    public bool MovingRight = true;
    public int MaxHealth = 100;
    private int _health;
    private Color _defaultColor;
    public GameObject ExplosionPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        _health = MaxHealth;
        _defaultColor = GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
        if (MovingRight)
        {
            transform.Translate(Vector2.right * Speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * Speed * Time.deltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        AudioManager.Instance.Play("Hit");
        _health -= damage;
        StartCoroutine(HitAnimation());


        float trauma = 0.5f;

        Camera.main.GetComponent<CameraMove>().AddTrauma(trauma);
        _health -= damage;

        if (_health <= 0)
        {
            Die();
        }
    }

    IEnumerator HitAnimation()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.white;

            yield return new WaitForSeconds(0.05f);

            spriteRenderer.color = _defaultColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void Die()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Flip();
        }
    }

    void Flip()
    {
        MovingRight = !MovingRight;
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}