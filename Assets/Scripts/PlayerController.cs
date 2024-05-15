using System;
using MyGridSystem;
using MyTiles;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private bool isGrounded = false;
    private float nextFire = 0.0f;
    private bool facingRight = true;
    public float Speed = 5.0f;
    public float BulletSpeed = 10.0f;
    public float JumpForce = 5.0f;
    public GameObject BulletPrefab;
    public Transform BulletSpawnPoint;
    public float FireRate = 0.5f;
    public float RecoilForce = 2f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (GetComponent<GridTileBase>().CurrentState.GameMode == GameMode.EditorMode)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            rb.gravityScale = 0;
        }
    }

    private void Update()
    {
        if (GetComponent<GridTileBase>().CurrentState.GameMode == GameMode.EditorMode)
        {
            return;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
        }

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + FireRate;
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        if (GetComponent<GridTileBase>().CurrentState.GameMode == GameMode.EditorMode)
        {
            return;
        }

        GetComponent<BoxCollider2D>().enabled = true;
        rb.gravityScale = 1;
        var moveHorizontal = Input.GetAxis("Horizontal");
        transform.position = new Vector2(transform.position.x + moveHorizontal * Speed * Time.deltaTime,
            transform.position.y);
        int layerMask = 1 << LayerMask.NameToLayer("Wall");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, layerMask);
        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }


        if (moveHorizontal > 0 && !facingRight || moveHorizontal < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Shoot()
    {
        int bulletCount = 3;
        for (int i = 0; i < bulletCount; i++)
        {
            var spawnPosition = BulletSpawnPoint.position +
                                (facingRight
                                    ? new Vector3(0, i * 0.1f, 0)
                                    : new Vector3(0, (bulletCount - 1 - i) * 0.1f, 0));
            var bullet = Instantiate(BulletPrefab, spawnPosition, Quaternion.identity);

            var bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            if (bulletRigidbody != null)
            {
                var direction = facingRight ? 1 : -1;
                bulletRigidbody.velocity = new Vector2(direction * BulletSpeed, 0.0f);

                bulletRigidbody.velocity =
                    Quaternion.Euler(0, 0, (i - bulletCount / 3) * 10) * bulletRigidbody.velocity;
            }

            var recoil = (facingRight ? Vector2.left : Vector2.right) * RecoilForce;
            transform.position = new Vector3(transform.position.x + recoil.x, transform.position.y + recoil.y,
                transform.position.z);
            AudioManager.Instance.Play("Shoot");
        }
    }
}