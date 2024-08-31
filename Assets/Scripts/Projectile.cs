using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;
    private BoxCollider2D boxCollider;
    private Animator anim;
    private float lifetime = 5f;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (hit) return;

        // Di chuyển viên đạn dựa vào hướng và tốc độ
        float movementSpeed = Time.deltaTime * speed * direction;
        transform.Translate(movementSpeed, 0, 0);

        // Kiểm tra thời gian tồn tại của viên đạn
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit) return;

        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");

        // Dừng tất cả các coroutine đang chạy
        StopAllCoroutines();
    }

    private void Deactivate()
    {
        Destroy(gameObject);
    }

    public void SetDirection(float _direction)
    {
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        // Thay đổi chiều rộng của hình ảnh theo hướng
        Vector3 localScale = transform.localScale;
        if (Mathf.Sign(localScale.x) != Mathf.Sign(_direction))
        {
            localScale.x = -localScale.x;
        }
        transform.localScale = localScale;

        // Đặt lại thời gian tồn tại của viên đạn
        lifetime = 5f;
    }
}
