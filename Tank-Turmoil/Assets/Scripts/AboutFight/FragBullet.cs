using UnityEngine;

public class FragBullet : Bullet
{
    [SerializeField] AudioClip fragBombSound;  // ��Ƭ�ӵ���ը��Ч
    private Shoot owner;
    private float lifeTime;
    private float timer;
    public GameObject fragmentPrefab;   // ��Ƭ�ӵ�Ԥ����
    public int fragmentCount = 12;      // ��ը�����ɵ���Ƭ����
    public float fragmentSpeed = 8f;    // ��Ƭ�ٶ�

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
                // ����Ƕȣ�0-360�ȣ�
                float angle = Random.Range(0f, 360f);
                Quaternion rot = Quaternion.Euler(0, 0, angle);
                Vector3 dir = rot * Vector3.up;

                GameObject frag = Instantiate(fragmentPrefab, transform.position, rot);
                Rigidbody2D rb = frag.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // ����ٶȣ�����Ƭ�ɵø���Ȼ
                    float randomSpeed = fragmentSpeed * Random.Range(0.5f, 1.5f);
                    rb.velocity = dir * randomSpeed;
                }

                // ������ʱ�䣨����Ƭ��ʧҲ��һ����
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
