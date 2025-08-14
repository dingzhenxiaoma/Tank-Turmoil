using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;   // 行进速度
    [SerializeField] float turnSpeed = 10f;  // 转向速度（越大越快）

    Animator anim;
    Rigidbody2D rb;

    bool dead = false;  //是否死亡
    Vector2 movement;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (dead)
        {
            movement = Vector2.zero;
            //anim.SetFloat("velocity", 0);
            return;
        }

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveHorizontal, moveVertical).normalized;

        //anim.SetFloat("velocity", movement.magnitude);

        if (movement.sqrMagnitude > 0.001f) // 有输入时才转向
        {
            float targetAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg-90;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = movement * moveSpeed;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果碰到的物体带有 "Bullet" 标签
        if (collision.gameObject.CompareTag("Bullet") && !dead)
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;
        rb.velocity = Vector2.zero; // 停止移动

        // 播放死亡动画（确保 Animator 中有 "die" 触发器）
        anim.SetTrigger("die");

        // 你可以在动画结束后销毁玩家
        Destroy(gameObject, 1.5f); // 延迟销毁，等待动画播放完成
    }
}
