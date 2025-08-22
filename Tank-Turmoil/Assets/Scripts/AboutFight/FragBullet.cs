using UnityEngine;

public class FragBullet : Bullet
{
    [SerializeField] AudioClip fragBombSound;  // 碎片子弹爆炸音效
    private Shoot owner;
    private float lifeTime;
    private float timer;
    public GameObject fragmentPrefab;   // 碎片子弹预制体
    public int fragmentCount = 12;      // 爆炸后生成的碎片数量
    public float fragmentSpeed = 8f;    // 碎片速度

    public void Init(Shoot shooter, float maxLife)
    {
        owner = shooter;
        lifeTime = maxLife;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Dead();
        }
    }

    public override void Dead()
    {
        Explode();
        owner.DisableFragMode();
        base.Dead();
    }

    public void Explode()
    {
        if (fragmentPrefab != null)
        {
            for (int i = 0; i < fragmentCount; i++)
            {
                // 随机角度（0-360度）
                float angle = Random.Range(0f, 360f);
                Quaternion rot = Quaternion.Euler(0, 0, angle);
                Vector3 dir = rot * Vector3.up;

                GameObject frag = Instantiate(fragmentPrefab, transform.position, rot);
                Rigidbody2D rb = frag.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // 随机速度，让碎片飞得更自然
                    float randomSpeed = fragmentSpeed * Random.Range(0.5f, 1.5f);
                    rb.velocity = dir * randomSpeed;
                }

                // 随机存活时间（让碎片消失也不一样）
                float life = Random.Range(1.5f, 3f);
                Destroy(frag, life);
            }
        }

        if (owner != null) owner.ClearFragReference();

        if (fragBombSound != null)
        {
            AudioSource.PlayClipAtPoint(fragBombSound, transform.position);
        }

        Destroy(gameObject);
        
    }

}
