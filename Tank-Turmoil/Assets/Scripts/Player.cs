using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerType { Player1, Player2 }   // �������
    [SerializeField] PlayerType playerType = PlayerType.Player1;

    [SerializeField] float moveSpeed = 6f;   // �ƶ��ٶ�
    [SerializeField] float turnSpeed = 150f; // ת���ٶȣ����ٶȣ���/s��
    [SerializeField] GameObject deathEffectPrefab; // ����������ЧԤ�Ƽ�

    [SerializeField] AudioClip deathSound; // ̹�˱�ը��Ч

    Animator anim;

    bool dead = false;
    bool canMove = true;
    float redMoveInput;    // W/S
    float redTurnInput;    // A/D

    float greenMoveInput;    // ����
    float greenTurnInput;    // ����

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

            greenMoveInput = 0;    // ����
            greenTurnInput = 0;    // ����
            return;
        }

        // ���ݲ�ͬ��Ҷ�ȡ����
        if (playerType == PlayerType.Player1)
        {
            // ���1: W/S ǰ�����ˣ�A/D ����
            /*redMoveInput = Input.GetAxisRaw("Vertical");
            redTurnInput = Input.GetAxisRaw("Horizontal");*/
            redMoveInput = (Input.GetKey(KeyCode.W) ? 1 : 0) +
                        (Input.GetKey(KeyCode.S) ? -1 : 0);

            redTurnInput = (Input.GetKey(KeyCode.D) ? 1 : 0) +
                        (Input.GetKey(KeyCode.A) ? -1 : 0);
        }
        else if (playerType == PlayerType.Player2)
        {
            // ���2: ����� ���� ǰ�����ˣ����� ����
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
            // A/D ������ת
            if (Mathf.Abs(redTurnInput) > 0.1f)
            {
                float rotationAmount = -redTurnInput * turnSpeed * Time.fixedDeltaTime;
                transform.Rotate(0, 0, rotationAmount);
            }

            // W/S �����ƶ�
            if (Mathf.Abs(redMoveInput) > 0.1f)
            {
                // �����Լ���ǰ���ƶ�
                transform.Translate(Vector3.up * redMoveInput * moveSpeed * Time.fixedDeltaTime, Space.Self);
            }
        }
        else if (playerType == PlayerType.Player2)
        {
            // J/L ������ת
            if (Mathf.Abs(greenTurnInput) > 0.1f)
            {
                float rotationAmount = -greenTurnInput * turnSpeed * Time.fixedDeltaTime;
                transform.Rotate(0, 0, rotationAmount);
            }

            // I/K �����ƶ�
            if (Mathf.Abs(greenMoveInput) > 0.1f)
            {
                // �����Լ���ǰ���ƶ�
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
                bullet.Dead(); // ���ӵ�����
            }
            Die(); // ̹������
        }
    }

    public void Die()
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
