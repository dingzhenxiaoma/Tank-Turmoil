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
    private AudioSource audioSource;

    [Header("Laser Settings")]
    [SerializeField] LineRenderer laserRenderer;   // 激光用的 LineRenderer
    [SerializeField] Material laserMaterial;       // 指定虚线材质
    [SerializeField] float dashPerUnit = 2.5f;     // 每单位长度虚线数量
    [SerializeField] float dashFrac = 0.6f;        // 每周期虚线占比
    [SerializeField] int maxReflections = 3;       // 最多折射次数
    [SerializeField] float maxLaserLength = 15f;   // 激光最大长度
    [SerializeField] LayerMask reflectMask;        // 可反射墙层
    [SerializeField] LayerMask damageMask;         // 可造成伤害目标层
    private bool laserFiring = false;  // 是否正在发射实线激光(按键检测)
    private bool isLaserFiring = false;  // 是否正在发射实线激光

    [Header("Frag Setting")]
    [SerializeField] GameObject fragBullet;    // 爆炸碎片子弹预制体
    GameObject currentFragBullet = null;  // 记录正在飞的Frag子弹

    [Header("Deathray Settings")]
    [SerializeField] LineRenderer deathrayRenderer;   // Deathray 光束
    [SerializeField] float deathrayMaxLength = 12f;  // 光束最大长度
    [SerializeField] float deathrayDuration = 3f;    // 光束存在的最长时间
    [SerializeField] float deathrayBendRadius = 5f;  // 光束末端的弯曲半径（感应敌人）
    [SerializeField] LayerMask playerMask;           // 检测玩家的层
    private bool isDeathrayFiring = false;
    private Coroutine deathrayRoutine;

    [Header("RCmissile Settings")]
    [SerializeField] GameObject RCmissileBullet;    // RCmissile子弹预制体
    GameObject currentRCmissile = null;


    private float timeSinceLastShot = 0f;
    private enum ModeType {normal,laser,frag,deathray, rcmissile }   // 发射类型
    private ModeType modeType=ModeType.normal;
    

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
                HandleNormalFire();
                break;

            case ModeType.laser:
                HandleLaserFire();
                break;

            case ModeType.frag:
                HandleFragFire();
                break;

            case ModeType.deathray:
                HandleDeathrayFire();
                break;
            case ModeType.rcmissile:
                HandleRCmissileFire();
                break;
        }
    }

    // =================== normal ===================
    private void HandleNormalFire()
    {
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
    }

    // =================== Frag ===================
    private void HandleFragFire()
    {
        if (playerType == PlayerType.Player1 && Input.GetKeyDown(KeyCode.Space) && ((currentFragBullet != null && timeSinceLastShot >= 0.2 * fireRate) || timeSinceLastShot >= fireRate))
        {
            HandleFrag();
            timeSinceLastShot = 0f;
        }
        else if (playerType == PlayerType.Player2 && Input.GetKeyDown(KeyCode.M) && ((currentFragBullet != null && timeSinceLastShot >= 0.2 * fireRate) || timeSinceLastShot >= fireRate))
        {
            HandleFrag();
            timeSinceLastShot = 0f;
        }
    }

    // =================== Laser ===================
    private void HandleLaserFire()
    {
        if (playerType == PlayerType.Player1) laserFiring = Input.GetKey(KeyCode.Space);
        else if (playerType == PlayerType.Player2) laserFiring = Input.GetKey(KeyCode.M);

        if (isLaserFiring)
            DrawLaser(isLaserFiring);
        else
            DrawLaser(laserFiring);
    }

    // =================== Deathray ===================
    private void HandleDeathrayFire()
    {
        bool keyDown = (playerType == PlayerType.Player1) ? Input.GetKey(KeyCode.Space) : Input.GetKey(KeyCode.M);

        if (keyDown)
        {
            if (!isDeathrayFiring)
            {
                // 开始发射
                deathrayRoutine = StartCoroutine(DeathrayRoutine());
            }
        }
        else
        {
            // 松开键
            if (isDeathrayFiring)
            {
                StopCoroutine(deathrayRoutine);
                DisableDeathrayMode();
            }
        }
    }

    // =================== RCmissile ===================
    private void HandleRCmissileFire()
    {
        // 玩家按下发射键才能发射导弹
        bool fireKey = (playerType == PlayerType.Player1) ? Input.GetKeyDown(KeyCode.Space) : Input.GetKeyDown(KeyCode.M);

        if (fireKey && currentRCmissile == null && timeSinceLastShot >= fireRate)
        {
            // 发射RCmissile
            currentRCmissile = Instantiate(RCmissileBullet, muzzlePosition.position, muzzlePosition.rotation);

            Player player = gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.DisableMove();
            }

            RCmissile rcScript = currentRCmissile.GetComponent<RCmissile>();
            rcScript.Init(this, bulletSpeed*0.5f, bulletLifeTime, playerType);  // 初始化参数

            timeSinceLastShot = 0f;
        }
    }

    // =================== 模式切换 ===================

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

    public void EnableFragMode()
    {
        modeType =ModeType.frag;
    }

    public void DisableFragMode()
    {
        modeType = ModeType.normal;
    }


    public void EnableDeathrayMode()
    {
        modeType = ModeType.deathray;
        if (deathrayRenderer != null) deathrayRenderer.enabled = false;
    }

    public void DisableDeathrayMode()
    {
        modeType = ModeType.normal;
        if (deathrayRenderer != null) deathrayRenderer.enabled = false;
        isDeathrayFiring = false;
    }

    public void EnableRCmissileMode()
    {
        modeType=ModeType.rcmissile;
    }
    public void DisableRCmissileMode()
    {
        modeType=ModeType.normal;
    }

    //=======================================================
    //=============== 具体实现 ==============================
    //=======================================================


    //====================== normal =========================
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


    //====================== laser =========================
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


    //====================== frag =========================
    private void HandleFrag()
    {
        if (currentFragBullet == null)
        {
            // 第一次按 → 发射普通子弹
            currentFragBullet = Instantiate(fragBullet, muzzlePosition.position, muzzlePosition.rotation);
            Rigidbody2D rb = currentFragBullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = muzzlePosition.up * bulletSpeed;

            // 给它加上 FragBullet 脚本
            FragBullet fragScript = currentFragBullet.GetComponent<FragBullet>();
            fragScript.Init(this, bulletLifeTime);

            if (shootSound != null)
            {
                audioSource.clip = shootSound;
                audioSource.Play();
            }
        }
        else
        {
            // 第二次按 → 触发爆炸
            FragBullet fragScript = currentFragBullet.GetComponent<FragBullet>();
            if (fragScript != null) fragScript.Explode();
            currentFragBullet = null;
            DisableFragMode();
        }
    }

    public void ClearFragReference()
    {
        currentFragBullet = null;  // 让 FragBullet 在爆炸或消失时回调
    }

    //====================== Deathray =========================
    IEnumerator DeathrayRoutine()
    {
        isDeathrayFiring = true;
        deathrayRenderer.enabled = true;

        float elapsed = 0f;
        float segmentLength = 0.1f;        // 每小段长度
        float totalLength = 0f;

        Player self = GetComponent<Player>();

        while (elapsed < deathrayDuration)
        {
            elapsed += Time.deltaTime;

            List<(Vector3, Vector3)> pointAndDir = new List<(Vector3, Vector3)>();
            pointAndDir.Add((muzzlePosition.position, muzzlePosition.up));
            totalLength = 0f;

            while (totalLength < deathrayMaxLength)
            {
                // 当前光束末端
                Vector3 lastPoint = pointAndDir[pointAndDir.Count - 1].Item1;
                Vector3 dir = pointAndDir[pointAndDir.Count - 1].Item2;

                // 计算下一段光束方向，默认沿上一次方向
                Vector3 nextPoint = lastPoint + dir * segmentLength;
                Vector3 nextDir = dir;

                // 检测弯曲：查找末端附近敌人
                Collider2D target = Physics2D.OverlapCircle(lastPoint, deathrayBendRadius, playerMask);
                if (target != null && target.CompareTag("Player"))
                {
                    Player targetPlayer = target.GetComponent<Player>();
                    if (targetPlayer != null && targetPlayer != self)
                    {
                        // 逐渐弯向敌人位置
                        Vector3 targetPos = target.transform.position;
                        Vector3 bendDir = (targetPos - lastPoint).normalized;
                        nextDir = (0.1f*bendDir+0.9f*dir).normalized;
                        nextPoint = lastPoint + nextDir * segmentLength;
                    }
                }
                pointAndDir.Add((nextPoint, nextDir));
                totalLength += segmentLength;
            }

            List<Vector3> points = new List<Vector3>();
            foreach (var pd in pointAndDir)
                points.Add(pd.Item1);

            // 更新 LineRenderer
            if (points.Count > 1)
            {
                deathrayRenderer.positionCount = points.Count;
                deathrayRenderer.SetPositions(points.ToArray());
            }

            if (elapsed <= Time.deltaTime * 2)
                continue;

            // --- 光束渲染完成后检测击中 ---
            for (int i = 0; i < pointAndDir.Count - 1; i++)
            {
                Vector3 a = pointAndDir[i].Item1;
                Vector3 b = pointAndDir[i + 1].Item1;
                RaycastHit2D hit = Physics2D.Linecast(a, b, playerMask);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    Player p = hit.collider.GetComponent<Player>();
                    if (p != null && p != self)
                    {
                        p.Die();
                        yield return new WaitForSeconds(0.5f);
                        DisableDeathrayMode();
                        timeSinceLastShot = 0f;
                    }
                }
            }

            yield return null;
        }
        DisableDeathrayMode();
        timeSinceLastShot = 0f;
    }


    //====================== RCmissile =========================
    public void ClearRCmissileReference()
    {
        currentRCmissile = null;
        DisableRCmissileMode(); // 回到 normal
    }


}
