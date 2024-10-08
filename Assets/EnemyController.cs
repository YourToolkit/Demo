using System.Collections;
using MyGridSystem;
using MyTiles;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float Speed = 2.0f;
    public bool MovingRight = true;
    public int MaxHealth = 100;
    private int _health;
    private Color _defaultColor;
    public GameObject ExplosionPrefab;
    [SerializeField] private float _wallDistance = 0.1f;
    private Rigidbody2D rb;
    public Transform RaycastOrigin;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (GetComponent<GridTileBase>().CurrentState.GameMode == GameMode.EditorMode)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            rb.gravityScale = 0;
            return;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        _health = MaxHealth;
        _defaultColor = GetComponentInChildren<SpriteRenderer>().color;
    }

    void Update()
    {
        if (GetComponent<GridTileBase>().CurrentState.GameMode == GameMode.EditorMode)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            rb.gravityScale = 0;
            return;
        }

        GetComponent<BoxCollider2D>().enabled = true;
        rb.gravityScale = 1;

        if (MovingRight)
        {
            transform.Translate(Vector2.right * Speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * Speed * Time.deltaTime);
        }

        int layerMask = 1 << LayerMask.NameToLayer("Wall");
        RaycastHit2D wallInfo =
            Physics2D.Raycast(RaycastOrigin.position, MovingRight ? Vector2.right : Vector2.left, _wallDistance,
                layerMask);

        if (wallInfo.collider != null && wallInfo.collider.CompareTag("Wall"))
        {
            Debug.Log("Wall");
            Flip();
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
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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


    void Flip()
    {
        MovingRight = !MovingRight;
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}