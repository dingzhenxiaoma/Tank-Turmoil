using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Shoot : MonoBehaviour
{
    public enum PlayerType { Player1, Player2 }   // �������
    [Header("PlayerType")]
    [SerializeField] PlayerType playerType = PlayerType.Player1;

    [Header("Prefabs")]
    [SerializeField] Transform muzzlePosition; // ǹ��λ��
    [SerializeField] GameObject projectile;    // �ӵ�Ԥ����

    [Header("Config")]
    [SerializeField] float fireRate = 0.5f;    // ���������룩
    [SerializeField] float bulletLifeTime = 3f; // �ӵ����ʱ��
    [SerializeField] float bulletSpeed = 10f;  // �ӵ��ٶ�

    [Header("Audio")]
    [SerializeField] AudioClip shootSound;     // �����Ч
    [SerializeField] AudioClip laserShootSound; // ���ⷢ����Ч

    [Header("Laser Settings")]
    [SerializeField] LineRenderer laserRenderer;   // �����õ� LineRenderer
    [SerializeField] Material laserMaterial;       // ָ�����߲���
    [SerializeField] float dashPerUnit = 2.5f;     // ÿ��λ������������
    [SerializeField] float dashFrac = 0.6f;        // ÿ��������ռ��
    [SerializeField] int maxReflections = 3;       // ����������
    [SerializeField] float maxLaserLength = 15f;   // ������󳤶�
    [SerializeField] LayerMask reflectMask;        // �ɷ���ǽ��
    [SerializeField] LayerMask damageMask;         // ������˺�Ŀ���

    [Header("Frag Setting")]
    [SerializeField] GameObject fragBullet;    // ��ը��Ƭ�ӵ�Ԥ����

    private float timeSinceLastShot = 0f;
    private enum ModeType {normal,laser,frag}   // ��������
    private ModeType modeType=ModeType.normal;
    private bool laserFiring = false;  // �Ƿ����ڷ���ʵ�߼���(�������)
    private bool isLaserFiring=false;  // �Ƿ����ڷ���ʵ�߼���

    GameObject currentFragBullet = null;  // ��¼���ڷɵ�Frag�ӵ�

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
                    // ��ͨ�ӵ�����
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
                    // ��������
                    if (playerType == PlayerType.Player1) laserFiring = Input.GetKey(KeyCode.Space);
                    else if (playerType == PlayerType.Player2) laserFiring = Input.GetKey(KeyCode.M);

                    if (isLaserFiring)
                        DrawLaser(isLaserFiring);
                    else
                        DrawLaser(laserFiring);
                    break;
                }
            case ModeType.frag:
                {
                    if (playerType == PlayerType.Player1 && Input.GetKeyDown(KeyCode.Space) &&((currentFragBullet!=null &&timeSinceLastShot>=0.2*fireRate)|| timeSinceLastShot >= fireRate))
                    {
                        HandleFrag();
                        timeSinceLastShot = 0f;
                    }
                    else if (playerType == PlayerType.Player2 && Input.GetKeyDown(KeyCode.M) && ((currentFragBullet != null && timeSinceLastShot >= 0.2 * fireRate) || timeSinceLastShot >= fireRate))
                    {
                        HandleFrag();
                        timeSinceLastShot = 0f;
                    }
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

    public void EnableFragMode()
    {
        modeType =ModeType.frag;
    }

    public void DisableFragMode()
    {
        modeType = ModeType.normal;
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

        // ���� LineRenderer
        laserRenderer.positionCount = points.Count;
        laserRenderer.SetPositions(points.ToArray());

        // ������ɫ
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
        
        // �������߲���
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

            // ������ɫ
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
        // ���ż��������Ч
        if (laserShootSound != null)
        {
            audioSource.clip = laserShootSound;
            audioSource.Play();
        }
        // �ȴ� 0.5 ��
        yield return new WaitForSeconds(0.5f); 
        DisableLaserMode();
        timeSinceLastShot = 0f;
        isLaserFiring = false;
    }


    private void HandleFrag()
    {
        if (currentFragBullet == null)
        {
            // ��һ�ΰ� �� ������ͨ�ӵ�
            currentFragBullet = Instantiate(fragBullet, muzzlePosition.position, muzzlePosition.rotation);
            Rigidbody2D rb = currentFragBullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = muzzlePosition.up * bulletSpeed;

            // �������� FragBullet �ű�
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
            // �ڶ��ΰ� �� ������ը
            FragBullet fragScript = currentFragBullet.GetComponent<FragBullet>();
            if (fragScript != null) fragScript.Explode();
            currentFragBullet = null;
            DisableFragMode();
        }
    }

    public void ClearFragReference()
    {
        currentFragBullet = null;  // �� FragBullet �ڱ�ը����ʧʱ�ص�
    }
}
