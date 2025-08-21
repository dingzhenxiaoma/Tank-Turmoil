using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Laser,Frag,Deathray,RCmissile,Gatling}   // ��������
    [SerializeField] ItemType itemType;
    [SerializeField] float lifeTime = 10f; // ���ߴ���ʱ��

    private void Start()
    {
        Destroy(gameObject, lifeTime); // һ��ʱ����Զ���ʧ
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Shoot shoot = collision.GetComponent<Shoot>();
        if (shoot != null)
        {
            switch(itemType)
            {
                case ItemType.Laser:
                    {
                        shoot.EnableLaserMode();
                        break;
                    }
                case ItemType.Frag:
                    {
                        shoot.EnableFragMode();
                        break;
                    }
                case ItemType.Deathray:
                    {
                        shoot.EnableDeathrayMode();
                        break;
                    }
                case ItemType.RCmissile:
                    {
                        shoot.EnableRCmissileMode();
                        break;
                    }
                case ItemType.Gatling:
                    {
                        shoot.EnableGatlingMode();
                        break;
                    }
                default:
                    break;
            }
            
            Destroy(gameObject); // ��ʰȡ�����ٵ���
        }
    }
}
