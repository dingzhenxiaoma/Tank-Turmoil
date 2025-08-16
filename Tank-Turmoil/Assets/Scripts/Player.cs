using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;   // 移动速度
    [SerializeField] float turnSpeed = 150f; // 转向速度（角速度，°/s）
    [SerializeField] GameObject deathEffectPrefab; // 死亡粒子特效预制件
    [SerializeField] AudioClip deathSound; // 坦克爆炸音效

    Animator anim;

    bool dead = false;
    float moveInput;    // W/S
    float turnInput;    // A/D

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (dead)
        {
            moveInput = 0;
            turnInput = 0;
            return;
        }

        // 获取输入
        moveInput = Input.GetAxisRaw("Vertical");   // W=1, S=-1
        turnInput = Input.GetAxisRaw("Horizontal"); // A=-1, D=1
    }

    private void FixedUpdate()
    {
        if (dead) return;

        // A/D 控制旋转
        if (Mathf.Abs(turnInput) > 0.1f)
        {
            float rotationAmount = -turnInput * turnSpeed * Time.fixedDeltaTime;
            transform.Rotate(0, 0, rotationAmount);
        }

        // W/S 控制移动
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            // 沿着自己的前方移动
            transform.Translate(Vector3.up * moveInput * moveSpeed * Time.fixedDeltaTime, Space.Self);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && !dead)
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;
        // 销毁坦克
        Destroy(gameObject);

        // 播放死亡音效
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // 生成死亡粒子特效
        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, transform.rotation);
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effect, 2f);
            }
        }
    }
}
