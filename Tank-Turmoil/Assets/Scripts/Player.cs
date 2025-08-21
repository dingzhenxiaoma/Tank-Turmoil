using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerType { Player1, Player2 }   // 玩家类型
    [SerializeField] PlayerType playerType = PlayerType.Player1;

    [SerializeField] float moveSpeed = 6f;   // 移动速度
    [SerializeField] float turnSpeed = 150f; // 转向速度（角速度，°/s）
    [SerializeField] GameObject deathEffectPrefab; // 死亡粒子特效预制件

    [SerializeField] AudioClip deathSound; // 坦克爆炸音效

    Animator anim;

    bool dead = false;
    bool canMove = true;
    float redMoveInput;    // W/S
    float redTurnInput;    // A/D

    float greenMoveInput;    // 上下
    float greenTurnInput;    // 左右

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (dead||!canMove)
        {
            redMoveInput=0;    // W/S
            redTurnInput = 0;    // A/D

            greenMoveInput = 0;    // 上下
            greenTurnInput = 0;    // 左右
            return;
        }

        // 根据不同玩家读取输入
        if (playerType == PlayerType.Player1)
        {
            // 玩家1: W/S 前进后退，A/D 左右
            /*redMoveInput = Input.GetAxisRaw("Vertical");
            redTurnInput = Input.GetAxisRaw("Horizontal");*/
            redMoveInput = (Input.GetKey(KeyCode.W) ? 1 : 0) +
                        (Input.GetKey(KeyCode.S) ? -1 : 0);

            redTurnInput = (Input.GetKey(KeyCode.D) ? 1 : 0) +
                        (Input.GetKey(KeyCode.A) ? -1 : 0);
        }
        else if (playerType == PlayerType.Player2)
        {
            // 玩家2: 方向键 ↑↓ 前进后退，←→ 左右
            greenMoveInput = (Input.GetKey(KeyCode. I) ? 1 : 0) +
                        (Input.GetKey(KeyCode.K) ? -1 : 0);

            greenTurnInput = (Input.GetKey(KeyCode.L) ? 1 : 0) +
                        (Input.GetKey(KeyCode.J) ? -1 : 0);
        }
    }

    private void FixedUpdate()
    {
        if (dead) return;

        if (playerType == PlayerType.Player1)
        {
            // A/D 控制旋转
            if (Mathf.Abs(redTurnInput) > 0.1f)
            {
                float rotationAmount = -redTurnInput * turnSpeed * Time.fixedDeltaTime;
                transform.Rotate(0, 0, rotationAmount);
            }

            // W/S 控制移动
            if (Mathf.Abs(redMoveInput) > 0.1f)
            {
                // 沿着自己的前方移动
                transform.Translate(Vector3.up * redMoveInput * moveSpeed * Time.fixedDeltaTime, Space.Self);
            }
        }
        else if (playerType == PlayerType.Player2)
        {
            // J/L 控制旋转
            if (Mathf.Abs(greenTurnInput) > 0.1f)
            {
                float rotationAmount = -greenTurnInput * turnSpeed * Time.fixedDeltaTime;
                transform.Rotate(0, 0, rotationAmount);
            }

            // I/K 控制移动
            if (Mathf.Abs(greenMoveInput) > 0.1f)
            {
                // 沿着自己的前方移动
                transform.Translate(Vector3.up * greenMoveInput * moveSpeed * Time.fixedDeltaTime, Space.Self);
            }
        }
    }

    public void EnableMove()
    {
        canMove = true;
    }
    public void DisableMove()
    {
        canMove=false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && !dead)
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Dead(); // 让子弹销毁
            }
            Die(); // 坦克死亡
        }
    }

    public void Die()
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
