using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Shoot : MonoBehaviour
{
    public enum PlayerType { Player1, Player2 }   // 玩家类型
    [Header("PlayerType")]
    [SerializeField] PlayerType playerType = PlayerType.Player1;

    [Header("Prefabs")]
    [SerializeField] Transform muzzlePosition; // 枪口位置
    [SerializeField] GameObject projectile;    // 子弹预制体

    [Header("Config")]
    [SerializeField] float fireRate = 0.5f;    // 射击间隔（秒）
    [SerializeField] float bulletLifeTime = 3f; // 子弹存活时间
    [SerializeField] float bulletSpeed = 10f;  // 子弹速度

    [Header("Audio")]
    [SerializeField] AudioClip shootSound;     // 射击音效
    [SerializeField] AudioClip laserShootSound; // 激光发射音效

    [Header("Laser Settings")]
    [SerializeField] LineRenderer laserRenderer;   // 激光用的 LineRenderer
    [SerializeField] Material laserMaterial;       // 指定虚线材质
    [SerializeField] float dashPerUnit = 2.5f;     // 每单位长度虚线数量
    [SerializeField] float dashFrac = 0.6f;        // 每周期虚线占比
    [SerializeField] int maxReflections = 3;       // 最多折射次数
    [SerializeField] float maxLaserLength = 15f;   // 激光最大长度
    [SerializeField] LayerMask reflectMask;        // 可反射墙层
    [SerializeField] LayerMask damageMask;         // 可造成伤害目标层

    private float timeSinceLastShot = 0f;
    private enum ModeType {normal,laser}   // 玩家类型
    private ModeType modeType=ModeType.normal;
    private bool laserFiring = false;  // 是否正在发射实线激光(按键检测)
    private bool isLaserFiring=false;  // 是否正在发射实线激光

    private AudioSource audioSource;

    private void Start()
    {
        timeSinceLastShot = fireRate;
        if (laserRenderer != null) laserRenderer.positionCount = 0;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        switch (modeType)
        {
            case ModeType.normal:
                {
                    // 普通子弹发射
                    if (playerType == PlayerType.Player1 && Input.GetKey(KeyCode.Space) && timeSinceLastShot >= fireRate)
                    {
                        Fire();
                        timeSinceLastShot = 0f;
                    }
                    else if (playerType == PlayerType.Player2 && Input.GetKey(KeyCode.M) && timeSinceLastShot >= fireRate)
                    {
                        Fire();
                        timeSinceLastShot = 0f;
                    }
                    break;
                }
            case ModeType.laser:
                {
                    // 检测射击键
                    if (playerType == PlayerType.Player1) laserFiring = Input.GetKey(KeyCode.Space);
                    else if (playerType == PlayerType.Player2) laserFiring = Input.GetKey(KeyCode.M);

                    if (isLaserFiring)
                        DrawLaser(isLaserFiring);
                    else
                        DrawLaser(laserFiring);
                    break;
                }
        }
    }

    public void EnableLaserMode()
    {
        modeType = ModeType.laser;
        if (laserRenderer != null) laserRenderer.enabled = true;
    }

    public void DisableLaserMode()
    {
        modeType = ModeType.normal;
        if (laserRenderer != null) laserRenderer.enabled = false;
    }

    private void Fire()
    {
        //if (shootSound != null) AudioSource.PlayClipAtPoint(shootSound, transform.position);

        if (shootSound != null)
        {
            audioSource.clip = shootSound;
            audioSource.Play();
        }

        GameObject bullet = Instantiate(projectile, muzzlePosition.position, muzzlePosition.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = muzzlePosition.up * bulletSpeed;

        Destroy(bullet, bulletLifeTime);
    }

    private void DrawLaser(bool solid)
    {
        if (laserRenderer == null) return;

        Vector2 pos = muzzlePosition.position;
        Vector2 dir = muzzlePosition.up;
        float remainLength = maxLaserLength;

        List<Vector3> points = new List<Vector3>();
        points.Add(pos);

        for (int i = 0; i <= maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos, dir, remainLength, reflectMask | damageMask);
            if (hit.collider != null)
            {
                points.Add(hit.point);

                if(solid)
                {
                    if ( ((1 << hit.collider.gameObject.layer) & damageMask) != 0)
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            var p = hit.collider.GetComponent<Player>();
                            if (p != null) p.Die();
                        }
                    }
                } 

                if (((1 << hit.collider.gameObject.layer) & reflectMask) != 0)
                {
                    dir = Vector2.Reflect(dir, hit.normal);
                    pos = hit.point + dir * 0.01f;
                    remainLength -= hit.distance;
                }
                else break;
            }
            else
            {
                points.Add(pos + dir * remainLength);
                break;
            }
        }

        // 更新 LineRenderer
        laserRenderer.positionCount = points.Count;
        laserRenderer.SetPositions(points.ToArray());

        // 设置颜色
        switch (playerType)
        {
            case PlayerType.Player1:
                {
                    laserRenderer.startColor =  Color.red ; 
                    break;
                }
            case PlayerType.Player2:
                {
                    laserRenderer.endColor = Color.green;
                    break;
                }
            default:
                break;
        }
        
        // 设置虚线材质
        if (laserMaterial != null)
        {
            float totalLen = 0f;
            for (int i = 1; i < points.Count; i++)
                totalLen += Vector3.Distance(points[i - 1], points[i]);
            totalLen = Mathf.Max(0.01f, totalLen);

            float periodCount = Mathf.Max(1f, totalLen * dashPerUnit);
            float period = 1f / periodCount;
            float dashSizeUV = solid ? 1f : period * dashFrac;
            float gapSizeUV = solid ? 0f : period * (1f - dashFrac);

            laserMaterial.SetFloat("_DashSize", dashSizeUV);
            laserMaterial.SetFloat("_GapSize", gapSizeUV);

            // 设置颜色
            switch (playerType)
            {
                case PlayerType.Player1:
                    {
                        laserMaterial.color = Color.red;
                        break;
                    }
                case PlayerType.Player2:
                    {
                        laserMaterial.color = Color.green;
                        break;
                    }
                default:
                    break;
            }
            laserRenderer.material = laserMaterial;


            if (solid)
            {
                StartCoroutine(SleepDisableLaserMode());
            }
        }
    }

    IEnumerator SleepDisableLaserMode()
    {
        isLaserFiring = true;
        // 播放激光射击音效
        if (laserShootSound != null)
        {
            audioSource.clip = laserShootSound;
            audioSource.Play();
        }
        // 等待 0.5 秒
        yield return new WaitForSeconds(0.5f); 
        DisableLaserMode();
        timeSinceLastShot = 0f;
        isLaserFiring = false;
    }

}
