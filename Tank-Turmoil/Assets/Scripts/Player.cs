using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;   // �н��ٶ�
    [SerializeField] float turnSpeed = 10f;  // ת���ٶȣ�Խ��Խ�죩

    Animator anim;
    Rigidbody2D rb;

    bool dead = false;  //�Ƿ�����
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

        if (movement.sqrMagnitude > 0.001f) // ������ʱ��ת��
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
        // ���������������� "Bullet" ��ǩ
        if (collision.gameObject.CompareTag("Bullet") && !dead)
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;
        rb.velocity = Vector2.zero; // ֹͣ�ƶ�

        // ��������������ȷ�� Animator ���� "die" ��������
        anim.SetTrigger("die");

        // ������ڶ����������������
        Destroy(gameObject, 1.5f); // �ӳ����٣��ȴ������������
    }
}
