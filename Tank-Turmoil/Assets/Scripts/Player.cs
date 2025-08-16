using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;   // �ƶ��ٶ�
    [SerializeField] float turnSpeed = 150f; // ת���ٶȣ����ٶȣ���/s��
    [SerializeField] GameObject deathEffectPrefab; // ����������ЧԤ�Ƽ�
    [SerializeField] AudioClip deathSound; // ̹�˱�ը��Ч

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

        // ��ȡ����
        moveInput = Input.GetAxisRaw("Vertical");   // W=1, S=-1
        turnInput = Input.GetAxisRaw("Horizontal"); // A=-1, D=1
    }

    private void FixedUpdate()
    {
        if (dead) return;

        // A/D ������ת
        if (Mathf.Abs(turnInput) > 0.1f)
        {
            float rotationAmount = -turnInput * turnSpeed * Time.fixedDeltaTime;
            transform.Rotate(0, 0, rotationAmount);
        }

        // W/S �����ƶ�
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            // �����Լ���ǰ���ƶ�
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
        // ����̹��
        Destroy(gameObject);

        // ����������Ч
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // ��������������Ч
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
