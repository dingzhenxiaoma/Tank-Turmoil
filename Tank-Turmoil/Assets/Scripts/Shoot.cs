using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Transform muzzlePosition; // 枪口位置
    [SerializeField] GameObject projectile;    // 子弹预制体

    [Header("Config")]
    [SerializeField] float fireRate = 0.5f;    // 射击间隔（秒）
    [SerializeField] float bulletLifeTime = 3f; // 子弹存活时间
    [SerializeField] float bulletSpeed = 10f;  // 子弹速度

    private float timeSinceLastShot = 0f;

    private void Start()
    {
        timeSinceLastShot = fireRate; // 开始就可以立刻射击
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        // 按下 Shift 发射
        if (Input.GetKey(KeyCode.Space) && timeSinceLastShot >= fireRate)
        {
            Fire();
            timeSinceLastShot = 0f;
        }
    }

    private void Fire()
    {
        // 生成子弹
        GameObject bullet = Instantiate(projectile, muzzlePosition.position, muzzlePosition.rotation);

        // 如果子弹有 Rigidbody2D，让它朝枪口方向飞
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = muzzlePosition.up* bulletSpeed;
        }

        // 一定时间后销毁子弹
        Destroy(bullet, bulletLifeTime);
    }
}
