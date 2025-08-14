using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Transform muzzlePosition; // ǹ��λ��
    [SerializeField] GameObject projectile;    // �ӵ�Ԥ����

    [Header("Config")]
    [SerializeField] float fireRate = 0.5f;    // ���������룩
    [SerializeField] float bulletLifeTime = 3f; // �ӵ����ʱ��
    [SerializeField] float bulletSpeed = 10f;  // �ӵ��ٶ�

    private float timeSinceLastShot = 0f;

    private void Start()
    {
        timeSinceLastShot = fireRate; // ��ʼ�Ϳ����������
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        // ���� Shift ����
        if (Input.GetKey(KeyCode.Space) && timeSinceLastShot >= fireRate)
        {
            Fire();
            timeSinceLastShot = 0f;
        }
    }

    private void Fire()
    {
        // �����ӵ�
        GameObject bullet = Instantiate(projectile, muzzlePosition.position, muzzlePosition.rotation);

        // ����ӵ��� Rigidbody2D��������ǹ�ڷ����
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = muzzlePosition.up* bulletSpeed;
        }

        // һ��ʱ��������ӵ�
        Destroy(bullet, bulletLifeTime);
    }
}
